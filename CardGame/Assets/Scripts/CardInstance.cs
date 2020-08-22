using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardInstance : NetworkBehaviour , ClickableInterface
{
    public CardInfo card = null;
    public enum CardState { InHand, InPlay, InDeck, InDiscard, Selected };
    public CardState currState = CardState.InDeck;
    public GameObject CardBack;
    public PlayerManagerScript player;

    public void OnClick(MouseControls.GameZone i_zone)
    {
        if (player)
        {
            if (currState == CardState.InHand)
            {
                Debug.Log("Clicked on card in hand");
                player.RpcHideHighlightedCard();
                currState = CardState.Selected;
                player.CmdSetHeldCard(gameObject);
            }
            else if (currState == CardState.InPlay)
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
        if (player)
        {
            if (currState == CardState.InHand || currState == CardState.InPlay || currState == CardState.InDiscard)
            {
                player.RpcDisplayHighlightedCard(card);
            }
        }
    }

    public void OnRelease(MouseControls.GameZone i_zone)
    {
        if (player)
        {
            if (i_zone == MouseControls.GameZone.Play && player.m_myArea.transform.childCount <= 5)
            {
                currState = CardState.InPlay;
                player.RpcSetInPlay();
            }
            else if (i_zone == MouseControls.GameZone.MyDiscard && player.m_canDiscard)
            {
                player.m_myDiscard.DiscardCard(card);
                Destroy(this.gameObject);
            }
            else
            {
                currState = CardState.InHand;
                player.RpcSetInHand();
            }
        }
    }

    public void FlipCard(bool isFlipped)
    {
        if(isFlipped)
        {
            CardBack.SetActive(false);
        }
        else
        {
            CardBack.SetActive(true);
        }
    }

    public void LoadCardInfo(CardInfo info)
    {
        card = info;
    }
}
