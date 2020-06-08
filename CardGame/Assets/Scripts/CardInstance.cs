using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance : MonoBehaviour , ClickableInterface
{
    public enum CardState { InHand, InPlay, InDeck, InDiscard, Selected };
    public CardState currState = CardState.InDeck;

    public void OnClick()
    {
        if(currState == CardState.InHand)
        {
            Debug.Log("Clicked on card in hand");
            GameStateManager.instance.HideHighlightedCard();
            currState = CardState.Selected;
            transform.SetParent(transform.parent.transform.parent,false);
            GameStateManager.instance.SetHeldCard(gameObject);
        }
        else
        {
            Debug.Log("Clicked on card in play");
        }
    }

    public void OnHighlighted()
    {
        if (currState == CardState.InHand || currState == CardState.InPlay)
        {
            GameStateManager.instance.DisplayHighlightedCard(transform.GetComponentInParent<CardUI>().card);
        }
    }

    public void OnRelease()
    {
        currState = CardState.InHand;
        GameStateManager.instance.HeldCard.transform.position = new Vector3(0.0f,0.0f,0.0f);
        transform.SetParent(transform.parent.Find("MyHand"),false);
        GameStateManager.instance.SetHeldCard(null);
    }
}
