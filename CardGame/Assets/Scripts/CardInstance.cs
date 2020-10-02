using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardInstance : NetworkBehaviour, ClickableInterface
{
    public enum CardState { InHand, InPlay, InDeck, InDiscard, Selected };
    public enum CardColor { Red, Blue, Yellow};
    public enum CardSide { Left, Right, Straight, None};
    public enum CardHeight { Low, High, Mid, None};
    public enum StarColor { Red, Gray, Yellow, None };
    public enum CardType { Technique, Strike};


    private CardInfo m_card = null;
    private CardState m_currState = CardState.InDeck;
    private CardColor m_cardColor;
    private StarColor m_starColor;
    private CardSide m_cardSide;
    private CardType m_cardType;
    private CardHeight m_cardHeight;
    [SerializeField] private GameObject m_cardBack;
    private PlayerManagerScript m_player;
    private const int MAXPLAYEDCARDS = 5;
    private bool m_canPlay = false;
    private bool m_cantBeBlocked = false;
    private bool m_cantBeDodged = false;
    private bool m_cantBeCountered= false;
    private bool m_starSpent = false;

    private void Start()
    {
        m_player = NetworkClient.connection.identity.GetComponent<PlayerManagerScript>();
    }

    public void OnClick(MouseControls.GameZone i_zone)
    {
        if (m_player)
        {
            if (m_currState == CardState.InHand && hasAuthority)
            {
                m_player.HideHighlightedCard();
                m_currState = CardState.Selected;
                m_player.CmdSetHeldCard(gameObject, m_card);
            }
            else if (m_currState == CardState.InPlay)
            {
                Debug.Log("Clicked on card in play");
            }
            else
            {
                Debug.Log("Clicked");
            }
        }
    }

    public void OnHighlighted()
    {
        if (m_player && !m_player.IsHoldingACard())
        {
            if (!m_cardBack.activeSelf && (m_currState == CardState.InHand || m_currState == CardState.InPlay || m_currState == CardState.InDiscard))
            {
                m_player.DisplayHighlightedCard(m_card, m_canPlay);
            }
        }
    }

    public void OnRelease(MouseControls.GameZone i_zone)
    {
        if (m_player)
        {
            if (i_zone == MouseControls.GameZone.Play && m_player.GetPlayAreaCardCount() < MAXPLAYEDCARDS && m_player.CanPlayCards())
            {
                if (m_canPlay)
                {
                    m_currState = CardState.InPlay;
                    m_player.CmdSetInPlay();
                    CmdPlayCard();
                }
                else
                {
                    m_currState = CardState.InHand;
                    m_player.CmdSetInHand();
                    MusicManager.m_instance.PlayClickError();
                }
            }
            else if (i_zone == MouseControls.GameZone.MyDiscard && m_player.CanDiscard())
            {
                m_player.GetDiscardPile().CmdDiscardCard(m_card, this.gameObject);
            }
            else
            {
                m_currState = CardState.InHand;
                m_player.CmdSetInHand();              
            }
        }
    }

    public void FlipCard(bool isFlipped)
    {
        if (isFlipped)
        {
            m_cardBack.SetActive(true);
        }
        else
        {
            m_cardBack.SetActive(false);
        }
    }

    public void LoadCardInfo(CardInfo info)
    {
        m_card = info;

        if(info.cardSymbol.Contains("Red"))
        {
            m_cardColor = CardColor.Red;
        }
        else if(info.cardSymbol.Contains("Blue"))
        {
            m_cardColor = CardColor.Blue;
        }
        else if(info.cardSymbol.Contains("Yellow"))
        {
            m_cardColor = CardColor.Yellow;
        }

        if (info.starValue.Contains("Red"))
        {
            m_starColor = StarColor.Red;
        }
        else if (info.starValue.Contains("Gray"))
        {
            m_starColor = StarColor.Gray;
        }
        else if (info.starValue.Contains("Yellow"))
        {
            m_starColor = StarColor.Yellow;
        }
        else
        {
            m_starColor = StarColor.None;
        }

        if (info.cardSymbol.Contains("Left"))
        {
            m_cardSide = CardSide.Left;
        }
        else if (info.cardSymbol.Contains("Right"))
        {
            m_cardSide = CardSide.Right;
        }
        else if (info.cardSymbol.Contains("Straight"))
        {
            m_cardSide = CardSide.Straight;
        }
        else
        {
            m_cardSide = CardSide.None;
        }

        if (info.cardSymbol.Contains("High"))
        {
            m_cardHeight = CardHeight.High;
        }
        else if (info.cardSymbol.Contains("Low"))
        {
            m_cardHeight = CardHeight.Low;
        }
        else if (info.cardSymbol.Contains("Mid"))
        {
            m_cardHeight = CardHeight.Mid;
        }
        else
        {
            m_cardHeight = CardHeight.None;
        }

        if (info.cardType.Contains("Technique"))
        {
            m_cardType = CardType.Technique;
        }
        else if (info.cardType.Contains("Strike"))
        {
            m_cardType = CardType.Strike;
        }

        m_cantBeBlocked = info.m_cantBeBlocked;
        m_cantBeDodged = info.m_cantBeDodged;
        m_cantBeCountered = info.m_cantBeCountered;
    }

    public CardInfo GetCard()
    {
        return m_card;
    }

    public CardState GetCardState()
    {
        return m_currState;
    }

    public void SetCardState(CardState i_state)
    {
        m_currState = i_state;
    }

    [Command]
    public void CmdPlayCard()
    {
        RpcPlayCard();
    }

    [ClientRpc]
    public void RpcPlayCard()
    {
        m_starSpent = m_player.WasStarSpent();

        if (hasAuthority)
        {
            m_canPlay = false;
            transform.GetComponent<CardUI>().SetOutlineColor(false);
        }
     
        if (GameStateManager.m_instance.GetLastPlayedCard())
        {
            Invoke(m_card.cardName.Replace(" ", string.Empty) + "OnPlay", 0.0f);
            Response();
            return;
        }

        SetLastPlayedCard(this);
      
        Invoke(m_card.cardName.Replace(" ",string.Empty) + "OnPlay", 0.0f);
        m_player.SetStarSpent(false);
    }


    private void SetLastPlayedCard(CardInstance i_card)
    {
        GameStateManager.m_instance.SetLastPlayedCard(i_card);
    }

    public void Response()
    {
        GameStateManager.m_instance.SwapAttackingPlayer();

        if (hasAuthority)
        {

            if (m_card.cardName.Contains("Block"))
            {
                m_player.BlockDamage();
            }
            else if (m_card.cardName.Contains("Counter") || GameStateManager.m_instance.GetLastPlayedCard().GetColor() == CardColor.Yellow)
            {

            }
            else
            {
                m_player.DodgeDamage();
            }

            GameStateManager.m_instance.GetAttackingPlayer().SetAllUnplayable();
            GameStateManager.m_instance.GetAttackingPlayer().UpdatePlayable();
        }
        else
        {

            if (m_card.cardName.Contains("Block"))
            {
                m_player.GetOppPlayer().BlockDamage();
            }
            else if (m_card.cardName.Contains("Counter") || GameStateManager.m_instance.GetLastPlayedCard().GetColor() == CardColor.Yellow)
            {

            }
            else
            {
                m_player.GetOppPlayer().DodgeDamage();
            }

        }

        Invoke(GameStateManager.m_instance.GetLastPlayedCard().GetCard().cardName.Replace(" ", string.Empty) + "WithResponse", 0.0f);

        //Fixing respoding with blue to yellow
        if(m_cardColor == CardColor.Blue)
        {
            SetLastPlayedCard(this);
        }
        else
        {
            SetLastPlayedCard(null);
        }
    }

    public void NoResponse()
    {
        if(hasAuthority)
        {
            if(m_player.GetOppPlayer().IsDazed())
            {
                m_player.GetOppPlayer().SetIsDazed(false);
                m_player.GetOppPlayer().GetMyInfo().SetDazeStars(false);
            }

            m_player.GetOppPlayer().TakeDamage(m_card.cardDamage);

            if(GameStateManager.m_instance.WasRedStarPlayed())
            {
                m_player.AddStar(1);
               GameStateManager.m_instance.SetRedStarPlayed(false);
            }

            if(GameStateManager.m_instance.GetLastPlayedCard().GetHeight() == CardHeight.High && GameStateManager.m_instance.GetLastPlayedCard().GetColor() == CardColor.Blue)
            {
                m_player.GetOppPlayer().AddDaze(1);
            }
        }
        else
        {
            if (m_player.IsDazed())
            {
                m_player.SetIsDazed(false);
                m_player.GetMyInfo().SetDazeStars(false);
            }

            m_player.TakeDamage(m_card.cardDamage);

            if (GameStateManager.m_instance.WasRedStarPlayed())
            {
                m_player.GetOppPlayer().AddStar(1);
                GameStateManager.m_instance.SetRedStarPlayed(false);
            }

            if (GameStateManager.m_instance.GetLastPlayedCard().GetHeight() == CardHeight.High && GameStateManager.m_instance.GetLastPlayedCard().GetColor() == CardColor.Blue)
            {
                m_player.AddDaze(1);
            }
        }

        Invoke(m_card.cardName.Replace(" ", string.Empty) + "NoResponse", 0.0f);
        SetLastPlayedCard(null);
    }

    private void SwitchPriority()
    {
        //Because a card instance will always have the localPlayer as its player reference.
        if (hasAuthority)
        {
            m_player.SetCanPlayCards(false);
            m_player.SetAllUnplayable();
            m_player.GetOppPlayer().SetCanPlayCards(true);
            m_player.GetMyInfo().SetUnselected();
            m_player.GetOppPlayer().GetMyInfo().SetSelected();
            GameStateManager.m_instance.DisablePassButton();        
        }
        else
        {
            m_player.SetCanPlayCards(true);
            m_player.UpdatePlayable();
            m_player.GetOppPlayer().SetCanPlayCards(false);
            m_player.GetMyInfo().SetSelected();
            m_player.GetOppPlayer().GetMyInfo().SetUnselected();
            GameStateManager.m_instance.EnablePassButton();
        }
    }

    public bool CanPlay()
    {
        return m_canPlay;
    }

    public void SetCanPlay(bool i_val)
    {
        m_canPlay = i_val;
    }

    public CardColor GetColor()
    {
        return m_cardColor;
    }

    public CardHeight GetHeight()
    {
        return m_cardHeight;
    }

    public CardSide GetSide()
    {
        return m_cardSide;
    }

    public bool IsUnblockable()
    {
        return m_cantBeBlocked;
    }

    public bool IsUndodgeable()
    {
        return m_cantBeDodged;
    }

    public bool IsUncounterable()
    {
        return m_cantBeCountered;
    }

    //Below are the methods that corespond to the specific name of the card

    //Jab Methods
    private void JabOnPlay()
    {
        SwitchPriority();   
        Debug.LogError("Jab Played");
    }

    private void JabWithResponse()
    {       
        Debug.LogError("Jab with response");
    }

    private void JabNoResponse()
    {       
        Debug.LogError("Jab no response");
    }

    //Bear Hug methods
    private void BearHugOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Bear Hug Played");
    }

    private void BearHugWithResponse()
    {
        Debug.LogError("Bear Hug with response");
    }

    private void BearHugNoResponse()
    {
        Debug.LogError("Bear Hug no response");
        int val = 0;

        if (m_starSpent)
        {
            Debug.LogError("Powered up version of bear hug");
            val = 2;
        }

        if (hasAuthority)
        {
            m_player.GainHealth(val);
            m_player.GetOppPlayer().AddStar(-1);
        }
        else
        {
            m_player.GetOppPlayer().GainHealth(val);
            m_player.AddStar(-1);
        }
    }

    //Belly Bump methods
    private void BellyBumpOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Belly Bump Played");
    }

    private void BellyBumpWithResponse()
    {
        Debug.LogError("Belly Bump with response");
    }

    private void BellyBumpNoResponse()
    {
        Debug.LogError("Belly Bump no response");

        if (m_starSpent)
        {
            Debug.LogError("Powered up version of belly bump");
        }

        if (hasAuthority)
        {
            m_player.AddStar(1);
        }
        else
        {
            m_player.GetOppPlayer().AddStar(1);
        }
    }

    //Grizzly Swipes methods
    private void GrizzlySwipesOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Grizzly Swipes Played");
    }

    private void GrizzlySwipesWithResponse()
    {
        Debug.LogError("Grizzly Swipes with response");
    }

    private void GrizzlySwipesNoResponse()
    {
        Debug.LogError("Grizzly Swipes no response");
    }

    //Kodiak Smash methods
    private void KodiakSmashOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Kodiak Smash Played");
    }

    private void KodiakSmashWithResponse()
    {
        Debug.LogError("Kodiak Smash with response");
    }

    private void KodiakSmashNoResponse()
    {
        if (m_starSpent)
        {
            if(hasAuthority)
            {
                m_player.GetOppPlayer().AddStar(-1);
            }
            else
            {
                m_player.AddStar(-1);
            }
            Debug.LogError("Kodiak Smash powered up");
        }

        Debug.LogError("Kodiak Smash no response");

        if (hasAuthority)
        {
            m_player.AddStar(1);
        }
        else
        {
            m_player.GetOppPlayer().AddStar(1);
        }
    }

    //Psych Up methods
    private void PsychUpOnPlay()
    {
        //To do, switching not a fix
        //GameStateManager.m_instance.SwapAttackingPlayer();
        SwitchPriority();
        GameStateManager.m_instance.SetRedStarPlayed(true);
        Debug.LogError("Psych Up Played");
    }

    private void PsychUpWithResponse()
    {
        Debug.LogError("Psych Up with response");
    }

    private void PsychUpNoResponse()
    {
        Debug.LogError("Psych Up no response");

        if (m_starSpent)
        {
            if (hasAuthority)
            {
                //
            }
            else
            {
                //
            }
            Debug.LogError("Psych Up powered up");
        }

        if (hasAuthority)
        {
            m_player.ResetDaze();
        }
        else
        {
            m_player.GetOppPlayer().ResetDaze();
        }

        GameStateManager.m_instance.SetRedStarPlayed(false);
    }

    //Rising Uppercut methods
    private void RisingUppercutOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Rising Uppercut Played");
    }

    private void RisingUppercutWithResponse()
    {
        Debug.LogError("Rising Uppercut with response");
    }

    private void RisingUppercutNoResponse()
    {
        Debug.LogError("Rising Uppercut no response");

        if (m_starSpent)
        {
            if (hasAuthority)
            {
                //
            }
            else
            {
                //
            }
            Debug.LogError("Rising Uppercut powered up");
        }

        if (hasAuthority)
        {
            m_player.AddStar(1);
        }
        else
        {
            m_player.GetOppPlayer().AddStar(1);
        }
    }

    //Slip Counter methods
    private void SlipCounterOnPlay()
    {
        Debug.LogError("Slip Counter Played");

        if (m_starSpent)
        {
            m_card.cardDamage *= 2;
            Debug.LogError("Slip Counter powered up");
        }

        if (hasAuthority)
        {
            m_player.GetOppPlayer().TakeDamage(m_card.cardDamage);
            m_player.AddStar(1);
        }
        else
        {
            m_player.TakeDamage(m_card.cardDamage);
            m_player.GetOppPlayer().AddStar(1);
        }
    }

    private void SlipCounterWithResponse()
    {
        Debug.LogError("Slip Counter with response");
    }

    private void SlipCounterNoResponse()
    {
        Debug.LogError("Slip Counter no response");
    }

    //Squirrel Counter methods
    private void SquirrelCounterOnPlay()
    {
        Debug.LogError("Squirrel Counter Played");

        if (m_starSpent)
        {
            Debug.LogError("Squirrel Counter powered up");
        }

        if (hasAuthority)
        {
            m_player.GetOppPlayer().TakeDamage(m_card.cardDamage);
            m_player.GetOppPlayer().AddStar(-1);
        }
        else
        {
            m_player.TakeDamage(m_card.cardDamage);
            m_player.AddStar(-1);
        }
    }

    private void SquirrelCounterWithResponse()
    {
        Debug.LogError("Squirrel Counter with response");
    }

    private void SquirrelCounterNoResponse()
    {
        Debug.LogError("Squirrel Counter no response");
    }

    // Straight Lunge methods
    private void StraightLungeOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Straight Lunge Played");
    }

    private void StraightLungeWithResponse()
    {
        Debug.LogError("Straight Lunge with response");
    }

    private void StraightLungeNoResponse()
    {
        Debug.LogError("Straight Lunge no response");
        int val = 1;

        if (m_starSpent)
        {
            val = 2;
            Debug.LogError("Straight Lunge powered up");
        }

        if (hasAuthority)
        {
            m_player.GetOppPlayer().AddStar(-val);
        }
        else
        {
            m_player.AddStar(-val);
        }
    }

    // Taunt methods
    private void TauntOnPlay()
    {
        //To do, switching not a fix
        //GameStateManager.m_instance.SwapAttackingPlayer();

        if (m_starSpent)
        {
            if (hasAuthority)
            {
                //
            }
            else
            {
                //
            }
            Debug.LogError("Taunt powered up");
        }
        GameStateManager.m_instance.SetRedStarPlayed(true);
        SwitchPriority();
        Debug.LogError("Taunt Played");
    }

    private void TauntWithResponse()
    {
        Debug.LogError("Taunt with response");
    }

    private void TauntNoResponse()
    {
        GameStateManager.m_instance.SetRedStarPlayed(false);
        Debug.LogError("Taunt no response");
    }

    // Jolt Haymaker methods
    private void JoltHaymakerOnPlay()
    {
        if (m_starSpent)
        {
            Debug.LogError("Powered up version of jolt haymaker");
            m_cantBeBlocked = true;
        }

        SwitchPriority();
        Debug.LogError("Jolt Haymaker Played");
    }

    private void JoltHaymakerWithResponse()
    {
        Debug.LogError("Jolt Haymaker with response");
    }

    private void JoltHaymakerNoResponse()
    {
        Debug.LogError("Jolt Haymaker no response");

        if (hasAuthority)
        {
            m_player.GetOppPlayer().AddStar(-1);
        }
        else
        {
            m_player.AddStar(-1);
        }
    }

    // Choco Bar methods
    private void ChocoBarOnPlay()
    {
        //To do, switching not a fix
        //GameStateManager.m_instance.SwapAttackingPlayer();
        SwitchPriority();
        Debug.LogError("Choco Bar Played");
        GameStateManager.m_instance.SetRedStarPlayed(true);
    }

    private void ChocoBarWithResponse()
    {
        Debug.LogError("Choco Bar with response");
    }

    private void ChocoBarNoResponse()
    {
        int val;

        if(m_starSpent)
        {
            Debug.LogError("Powered up version of choco bar");
            val = 4;
        }
        else
        {
            val = 2;
        }

        if (hasAuthority)
        {
            m_player.GainHealth(val);
        }
        else
        {
            m_player.GetOppPlayer().GainHealth(val);
        }

        GameStateManager.m_instance.SetRedStarPlayed(false);
        Debug.LogError("Choco Bar no response");
    }

     //Fake Out methods
    private void FakeOutOnPlay()
    {
        SwitchPriority();
        Debug.LogError("Fake Out Played");
    }

    private void FakeOutWithResponse()
    {
        Debug.LogError("Fake Out with response");
    }

    private void FakeOutNoResponse()
    {
        Debug.LogError("Fake Out no response");

        if (m_starSpent)
        {
            Debug.LogError("Powered up version of fake out");
        }

        if (hasAuthority)
        {
            m_player.GetOppPlayer().AddStar(-1);
        }
        else
        {
            m_player.AddStar(-1);
        }
    }
}
