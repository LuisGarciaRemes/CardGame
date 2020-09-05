﻿using System.Collections;
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
    public enum CardType { Technique, Single_Strike, Multi_Strike};


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
                m_player.DisplayHighlightedCard(m_card);
            }
        }
    }

    public void OnRelease(MouseControls.GameZone i_zone)
    {
        if (m_player)
        {
            if (i_zone == MouseControls.GameZone.Play && m_player.GetPlayAreaCardCount() < MAXPLAYEDCARDS && m_player.CanPlayCards())
            {
                m_currState = CardState.InPlay;
                m_player.CmdSetInPlay();
                CmdPlayCard();
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
        else if (info.cardType.Contains("Single-Strike"))
        {
            m_cardType = CardType.Single_Strike;
        }
        else if (info.cardType.Contains("Multi-Strike"))
        {
            m_cardType = CardType.Multi_Strike;
        }

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
        GameStateManager.m_instance.SetLastPlayedCard(m_card);

        //Because a card instance will always have the localPlayer as its player reference.
        if(hasAuthority)
        {
            m_player.SetCanPlayCards(false);
            m_player.GetOppPlayer().SetCanPlayCards(true);
        }
        else
        {
            m_player.SetCanPlayCards(true);
            m_player.GetOppPlayer().SetCanPlayCards(false);
        }

        //Invoke(m_card.name, 0.0f);
    }

    //Below are the methods that corespond to the specific name of the card
}
