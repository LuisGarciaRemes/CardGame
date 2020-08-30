﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardInstance : NetworkBehaviour, ClickableInterface
{
    public enum CardState { InHand, InPlay, InDeck, InDiscard, Selected };
    public enum CardColor { Red, Blue, Yellow};
    public enum CardSide { Left, Right, Straight, None };
    public enum CardHight { Low, High, Mid, None };
    public enum StarColor { Red, Gray, Yellow, None };


    public CardInfo m_card = null;
    public CardState m_currState = CardState.InDeck;
    public CardColor m_cardColor;
    public StarColor m_starColor;
    public CardSide m_cardSide;
    public CardHight m_cardHight;
    public GameObject m_cardBack;
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
                Debug.Log("Clicked on card in hand");
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
        if (m_player && !m_player.m_myHeldCard)
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
            if (i_zone == MouseControls.GameZone.Play && m_player.m_myArea.transform.childCount < MAXPLAYEDCARDS)
            {
                m_currState = CardState.InPlay;
                m_player.CmdSetInPlay();
            }
            else if (i_zone == MouseControls.GameZone.MyDiscard && m_player.m_canDiscard)
            {
                m_player.m_myDiscard.CmdDiscardCard(m_card, this.gameObject);
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
        else if (info.starValue.Contains("None"))
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
            m_cardHight = CardHight.High;
        }
        else if (info.cardSymbol.Contains("Low"))
        {
            m_cardHight = CardHight.Low;
        }
        else if (info.cardSymbol.Contains("Mid"))
        {
            m_cardHight = CardHight.Mid;
        }
        else
        {
            m_cardHight = CardHight.None;
        }

    }


    public void PlayCard()
    {
        Invoke(m_card.name, 0.0f);
    }

    //Below are the methods that corespond to the specific name of the card
}
