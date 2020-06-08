using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public CardInfo card = null;
    [SerializeField] Image cardArt;
    [SerializeField] Text cardCostText;
    [SerializeField] Text cardValueText;
    [SerializeField] Text cardText;
    [SerializeField] Text cardTypeText;
    [SerializeField] Text cardNameText;

    private void Start()
    {
        if(card != null)
        {
            LoadCard(card);
        }
    }

    public void LoadCard(CardInfo i_card)
    {
        card = i_card;
        cardArt.sprite = card.art;
        cardCostText.text = card.cardCost.ToString();
        cardText.text = card.cardText;
        cardValueText.text = card.cardValue.ToString();
        cardTypeText.text = card.cardType;
        cardNameText.text = card.cardName;
    }

}
