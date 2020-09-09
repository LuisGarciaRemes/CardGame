using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class PlayerManagerScript : NetworkBehaviour
{
    [SerializeField] private List<CharacterStats> m_characters;
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject DiscardPrefab;
    [SerializeField] private GameObject CardPrefab;
    private Deck m_myDeck;
    private DiscardPile m_myDiscard;
    private Deck m_oppDeck;
    private DiscardPile m_oppDiscard;
    private bool m_canDiscard = false;
    private bool m_canDraw = false;
    private bool m_canPlay = false;

    private CardUI m_highlightedCard;
    private GameObject m_myHeldCard = null;
    private GameObject m_myHand;

    private PlayerManagerScript m_oppPlayer;
    private GameObject m_oppHand;
    private GameObject m_myArea;
    private GameObject m_oppArea;

    private int[] m_health = {0,0,0};
    private int m_currHealthIndex = 0;
    private int m_maxDaze = 0;
    private int m_currDaze = 0;
    private int m_maxStar = 0;
    private int m_currStar = 0;

    private PlayerInfo m_myInfo;
    private int m_deckID = -1;

    private int m_drawnThisRound = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (hasAuthority)
        {
            CmdCreateDeckAndDiscard(m_deckID = GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().charSelect.GetComponent<CharSelect>().GetDeckID());
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (hasAuthority)
        {
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().charSelect.SetActive(true);
        }
    }

    private void Start()
    {
        MouseControls.AddPlayerInGame();
    }

    [Command]
    public void CmdSpawnCard(Transform i_trans, CardInfo i_info)
    {
        GameObject temp = Instantiate(CardPrefab, i_trans);
        NetworkServer.Spawn(temp, connectionToClient);
        temp.GetComponent<CardInstance>().LoadCardInfo(i_info);
        temp.GetComponent<CardInstance>().SetCardState(CardInstance.CardState.Selected);
        temp.GetComponent<CardUI>().LoadCard(i_info);
        RpcSetHeldCard(temp, i_info);
    }

    public void DisplayHighlightedCard(CardInfo info)
    {
        if (!m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(true);

            if (!m_highlightedCard.transform.GetChild(0).gameObject.activeSelf)
            {
                for (int i = 0; i < 8; i++)
                {
                    m_highlightedCard.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if (info != m_highlightedCard.card)
        {
            m_highlightedCard.LoadCard(info);
        }
    }

    public void HideHighlightedCard()
    {
        if (m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(false);
        }
    }

    [Command]
    public void CmdSetHeldCard(GameObject i_card, CardInfo i_info)
    {
        RpcSetHeldCard(i_card, i_info);
    }

    [ClientRpc]
    public void RpcSetHeldCard(GameObject i_card, CardInfo i_info)
    {
        i_card.GetComponent<CardInstance>().LoadCardInfo(i_info);
        i_card.GetComponent<CardInstance>().SetCardState(CardInstance.CardState.Selected);
        i_card.GetComponent<CardUI>().LoadCard(i_info);

        if (hasAuthority)
        {
            i_card.transform.SetParent(m_myHand.transform.parent, false);
        }
        else
        {
            i_card.transform.SetParent(m_myHand.transform, false);
            i_card.GetComponent<CardInstance>().FlipCard(true);
        }
        i_card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        i_card.transform.localRotation = new Quaternion();
        m_myHeldCard = i_card;
    }

    [Command]
    public void CmdSetInPlay()
    {
        RpcSetInPlay();
    }

    [ClientRpc]
    public void RpcSetInPlay()
    {
        if (!hasAuthority)
        {
            m_myHeldCard.GetComponent<CardInstance>().FlipCard(false);
        }
        m_myHeldCard.GetComponent<CardInstance>().SetCardState(CardInstance.CardState.InPlay);
        m_myHeldCard.transform.SetParent(m_myArea.transform, false);
        m_myHeldCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        m_myHeldCard.transform.localRotation = new Quaternion();
        m_myHeldCard = null;
    }

    [Command]
    public void CmdSetInHand()
    {
        RpcSetInHand();
    }

    [ClientRpc]
    public void RpcSetInHand()
    {
        if (m_myHeldCard)
        {
            m_myHeldCard.GetComponent<CardInstance>().SetCardState(CardInstance.CardState.InHand);
            m_myHeldCard.transform.SetParent(m_myHand.transform, false);
            m_myHeldCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myHeldCard.transform.localRotation = new Quaternion();
            m_myHeldCard = null;
        }
    }


    [Command]
    public void CmdCreateDeckAndDiscard(int i_deckID)
    {
        GameObject overlay = GameObject.Find("Screen Space (World)");
        GameObject temp = Instantiate(DeckPrefab, overlay.transform);
        NetworkServer.Spawn(temp,connectionToClient);
        m_myDeck = temp.GetComponent<Deck>();
        m_deckID = i_deckID;
        m_myDeck.SetDeckList(m_characters[m_deckID].m_deckList);
        temp = Instantiate(DiscardPrefab, overlay.transform);
        NetworkServer.Spawn(temp, connectionToClient);
        m_myDiscard = temp.GetComponent<DiscardPile>();
        m_myHand = GameObject.Find("MyHand");
        m_oppHand = GameObject.Find("OppHand");
        m_myArea = GameObject.Find("MyArea");
        m_oppArea = GameObject.Find("OppArea");
        m_myInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        RpcSetMyPlayerReferences(m_myDeck.gameObject,m_myDiscard.gameObject,m_deckID);
    }

    [ClientRpc]
    public void RpcSetMyPlayerReferences(GameObject myDeck, GameObject myDiscard , int i_deckID)
    {
        if (hasAuthority)
        {
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_highlightedCard = GameObject.Find("HighlightedPrefab").GetComponent<CardUI>();
            m_myDeck = myDeck.GetComponent<Deck>();
            m_deckID = i_deckID;
            m_myDeck.SetDeckList(m_characters[m_deckID].m_deckList);
            m_myDeck.Shuffle();
            myDeck.name = "MyDeck";            
            m_myDiscard = myDiscard.GetComponent<DiscardPile>();
            myDiscard.name = "MyDiscard";
            m_myHand = GameObject.Find("MyHand");
            m_oppHand = GameObject.Find("OppHand");
            m_myArea = GameObject.Find("MyArea");
            m_oppArea = GameObject.Find("OppArea");
            m_myInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
            m_myInfo.UpdateCharacterStats(m_characters[i_deckID]);
            UpdateCharStats();
            m_myDeck.gameObject.transform.SetParent(overlay.transform, false);
            m_myDiscard.gameObject.transform.SetParent(overlay.transform, false);
            m_myDeck.gameObject.transform.localPosition = new Vector3(335.0f, -150.0f, 0.0f);
            m_myDiscard.gameObject.transform.localPosition = new Vector3(-335.0f, -150.0f, 0.0f);
            m_myDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.gameObject.name = "MyPlayer";
            GameObject.Find("Controls").GetComponent<MouseControls>().SetPlayer(this);
        }
    }

    [Command]
    public void CmdSetOpposingReferences()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int tempID = -1;

        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerManagerScript>() != this)
            {
                tempID = player.GetComponent<PlayerManagerScript>().m_deckID;
            }
        }

        if(GameStateManager.m_instance.FirstPlayerChosen() == false)
        {
            GameStateManager.m_instance.SetFirstPlayerChosen(true);

            switch(UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    GameStateManager.m_instance.SetAttackingPlayer(players[0].GetComponent<PlayerManagerScript>());
                    players[0].GetComponent<PlayerManagerScript>().RpcSetAsAttacker();
                    GameStateManager.m_instance.SetDefendingPlayer(players[1].GetComponent<PlayerManagerScript>());
                    players[1].GetComponent<PlayerManagerScript>().RpcSetAsDefender();

                    break;
                case 1:
                    GameStateManager.m_instance.SetAttackingPlayer(players[1].GetComponent<PlayerManagerScript>());
                    players[1].GetComponent<PlayerManagerScript>().RpcSetAsAttacker();
                    GameStateManager.m_instance.SetDefendingPlayer(players[0].GetComponent<PlayerManagerScript>());
                    players[0].GetComponent<PlayerManagerScript>().RpcSetAsDefender();
                    break;
                default:
                    break;
            }
        }
        RpcSetOppPlayerReferences(tempID);
    }

    [ClientRpc]
    public void RpcSetOppPlayerReferences(int i_deckID)
    {
        if (hasAuthority)
        {
            GameStateManager.m_instance.SetFirstPlayerChosen(true);
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_oppDeck = GameObject.Find("OppDeck(Clone)").GetComponent<Deck>();
            m_oppDiscard = GameObject.Find("OppDiscard(Clone)").GetComponent<DiscardPile>();
            m_oppDeck.gameObject.transform.SetParent(overlay.transform, false);
            m_oppDiscard.gameObject.transform.SetParent(overlay.transform, false);
            m_oppDeck.gameObject.transform.localPosition = new Vector3(-335.0f, 150.0f, 0.0f);
            m_oppDiscard.gameObject.transform.localPosition = new Vector3(335.0f, 150.0f, 0.0f);
            m_oppDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_oppDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_oppPlayer = GameObject.Find("OppPlayer(Clone)").GetComponent<PlayerManagerScript>();
            m_oppPlayer.m_myDeck = m_oppDeck;
            m_oppPlayer.m_myDiscard = m_oppDiscard;
            m_oppPlayer.m_oppDeck = m_myDeck;
            m_oppPlayer.m_oppDiscard = m_myDiscard;
            m_oppPlayer.m_myHand = m_oppHand;
            m_oppPlayer.m_myArea = m_oppArea;
            m_oppPlayer.m_oppArea = m_myArea;
            m_oppPlayer.m_deckID = i_deckID;
            m_oppPlayer.m_myDeck.SetDeckList(m_characters[i_deckID].m_deckList);
            m_oppPlayer.m_oppHand = m_myHand;
            m_oppPlayer.m_myInfo = GameObject.Find("OpponentInfo").GetComponent<PlayerInfo>();
            m_oppPlayer.m_myInfo.UpdateCharacterStats(m_characters[i_deckID]);
            m_oppPlayer.UpdateCharStats();
        }
        else
        {
            m_oppPlayer = GameObject.Find("MyPlayer").GetComponent<PlayerManagerScript>();
        }     
    }

    [Command]
    public void CmdPlayerIsReady()
    {
        RpcPlayerIsReady();
    }

    [ClientRpc]
    public void RpcPlayerIsReady()
    {
        MouseControls.AddPlayerReady();
    }

    private void UpdateCharStats()
    {
        for(int i = 0; i < 3; i++)
        {
            m_health[i] = m_characters[m_deckID].m_health[i];
        }

        m_maxDaze = m_characters[m_deckID].m_dazeVal;
        m_maxStar = m_characters[m_deckID].m_starVal;
    }

    public void TakeDamage(int i_value)
    {
        m_health[m_currHealthIndex] -= i_value;

        m_myInfo.UpdateHealth(m_health[m_currHealthIndex]);

        if (i_value > 0.0f)
        {
            m_myInfo.UnblockedPlayPunchAnimation();
        }

        if(m_health[m_currHealthIndex] <= 0)
        {
            //To do --- 1. Knock down stuff 2. Check if knock out
        }
    }

    public void GainHealth(int i_value)
    {
        m_health[m_currHealthIndex] += i_value;
        m_health[m_currHealthIndex] = Math.Min(m_health[m_currHealthIndex],m_characters[m_deckID].m_health[m_currHealthIndex]);
    }

    public void ShuffleDiscardIntoDeck()
    {
        //To do place discard into deck and shuffle
    }

    public  void AddDaze(int i_value)
    {
        m_currDaze += i_value;

        if(m_currDaze >= m_maxDaze)
        {
            m_currDaze = 0;
            //To do 1. Get dazed
        }
    }

    public void RemoveDaze(int i_value)
    {
        m_currDaze -= i_value;

        m_currDaze = Math.Max(m_currDaze,0);
    }

    public void AddStar()
    {
        m_currStar++;
        m_currStar = Math.Min(m_currStar,m_maxDaze);
    }

    public void RemoveStar()
    {
        m_currStar--;
        m_currStar = Math.Max(m_currStar, 0);
    }

    [Command]
    public void CmdSetAsAttacker()
    {
        GameStateManager.m_instance.SetAttackingPlayer(this);
        RpcSetAsAttacker();
    }

    [ClientRpc]
    public void RpcSetAsAttacker()
    {
        GameStateManager.m_instance.SetAttackingPlayer(this);
    }

    [Command]
    public void CmdSetAsDefender()
    {
        GameStateManager.m_instance.SetDefendingPlayer(this);
        RpcSetAsDefender();
    }

    [ClientRpc]
    public void RpcSetAsDefender()
    {
        GameStateManager.m_instance.SetDefendingPlayer(this);
    }

    public DiscardPile GetDiscardPile()
    {
        return m_myDiscard;
    }

    public DiscardPile GetOppDiscardPile()
    {
        return m_oppDiscard;
    }

    public bool CanDiscard()
    {
        return m_canDiscard;
    }

    public bool CanPlayCards()
    {
        return m_canPlay;
    }

    public bool CanDrawCards()
    {
        return m_canDraw;
    }


    public void SetCanPlayCards(bool i_bool)
    {
        m_canPlay = i_bool;
    }

    public bool IsHoldingACard()
    {
        return m_myHeldCard;
    }

    public GameObject GetHeldCard()
    {
        return m_myHeldCard;
    }

    public int GetPlayAreaCardCount()
    {
        return m_myArea.transform.childCount;
    }

    public int GetHandCardCount()
    {
        return m_myHand.transform.childCount;
    }

    public int GetCardsDrawn()
    {
        return m_drawnThisRound;
    }

    public void SetCardsDrawn(int i_val)
    {
        m_drawnThisRound = i_val;
    }

    public void SetCanDraw(bool i_bool)
    {
        m_canDraw = i_bool;
    }

    public Deck GetMyDeck()
    {
        return m_myDeck;
    }

    public Deck GetOppDeck()
    {
        return m_oppDeck;
    }

    public PlayerManagerScript GetOppPlayer()
    {
        return m_oppPlayer;
    }


    [Command]
    public void CmdPassTheTurn()
    {
        RpcPassTheTurn();
    }

    [ClientRpc]
    public void RpcPassTheTurn()
    {
        //Because a card instance will always have the localPlayer as its player reference.
        if (hasAuthority)
        {
            SetCanPlayCards(false);
            GetOppPlayer().SetCanPlayCards(true);
        }
        else
        {
            SetCanPlayCards(false);
            GetOppPlayer().SetCanPlayCards(true);
            GameStateManager.m_instance.EnablePassButton();
        }

        if (GameStateManager.m_instance.GetLastPlayedCard())
        {
            GameStateManager.m_instance.GetLastPlayedCard().NoResponse();
        }

        GameStateManager.m_instance.SetLastPlayedCard(null);

        if(this == GameStateManager.m_instance.GetAttackingPlayer())
        {
            GameStateManager.m_instance.SwapAttackingPlayer();
        }
    }
}

