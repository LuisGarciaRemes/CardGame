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
    private PlayerManagerScript player;

    private void Start()
    {
       player = NetworkClient.connection.identity.GetComponent<PlayerManagerScript>();
    }

    public void OnClick(MouseControls.GameZone i_zone)
    {
        if(currState == CardState.InHand)
        {
            Debug.Log("Clicked on card in hand");
            UIManager.instance.HideHighlightedCard();
            currState = CardState.Selected;
            transform.SetParent(transform.parent.transform.parent,false);
            UIManager.instance.SetHeldCard(gameObject);
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

    public void OnHighlighted()
    {
        if (currState == CardState.InHand || currState == CardState.InPlay || currState == CardState.InDiscard)
        {
            UIManager.instance.DisplayHighlightedCard(card);
        }
    }

    public void OnRelease(MouseControls.GameZone i_zone)
    {
        if (i_zone == MouseControls.GameZone.Play && UIManager.instance.m_myArea.transform.childCount <= 5)
        {
            currState = CardState.InPlay;       
            UIManager.instance.m_heldCard.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            transform.SetParent(UIManager.instance.m_myArea.transform, false);
            UIManager.instance.SetHeldCard(null);
        }
        else if(i_zone == MouseControls.GameZone.MyDiscard && player.m_canDiscard)
        {
            player.m_myDiscard.DiscardCard(card);
            Destroy(this.gameObject);
        }
        else
        {
            currState = CardState.InHand;
            UIManager.instance.m_heldCard.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            transform.SetParent(UIManager.instance.m_myHand.transform, false);
            UIManager.instance.SetHeldCard(null);
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
