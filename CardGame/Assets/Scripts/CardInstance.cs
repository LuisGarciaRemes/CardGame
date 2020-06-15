using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance : MonoBehaviour , ClickableInterface
{
    public CardInfo card = null;
    public enum CardState { InHand, InPlay, InDeck, InDiscard, Selected };
    public CardState currState = CardState.InDeck;
    private GameObject CardBack;

    private void Start()
    {
        CardBack = transform.Find("CardBack").gameObject;
    }

    public void OnClick(MouseControls.GameZone i_zone)
    {
        if(currState == CardState.InHand)
        {
            Debug.Log("Clicked on card in hand");
            GameStateManager.instance.HideHighlightedCard();
            currState = CardState.Selected;
            transform.SetParent(transform.parent.transform.parent,false);
            GameStateManager.instance.SetHeldCard(gameObject);
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
            GameStateManager.instance.DisplayHighlightedCard(card);
        }
    }

    public void OnRelease(MouseControls.GameZone i_zone)
    {
        if (i_zone == MouseControls.GameZone.Play && GameStateManager.instance.myPlayArea.transform.childCount <= 5)
        {
            currState = CardState.InPlay;
            GameStateManager.instance.HeldCard.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            transform.SetParent(GameStateManager.instance.myPlayArea.transform, false);
            GameStateManager.instance.SetHeldCard(null);
        }
        else if(i_zone == MouseControls.GameZone.MyDiscard && GameStateManager.instance.canDiscard)
        {
            GameStateManager.instance.MyDiscard.DiscardCard(card);
            Destroy(this.gameObject);
        }
        else
        {
            currState = CardState.InHand;
            GameStateManager.instance.HeldCard.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            transform.SetParent(GameStateManager.instance.myHand.transform, false);
            GameStateManager.instance.SetHeldCard(null);
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
