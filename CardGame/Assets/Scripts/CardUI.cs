using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardUI : MonoBehaviour
{
    [SerializeField] Image cardArt;
    [SerializeField] Text cardCostText;
    [SerializeField] Text cardValueText;
    [SerializeField] Text cardText;
    [SerializeField] Text cardTypeText;
    [SerializeField] Text cardNameText;
    internal CardInfo card;

    private void Start()
    {
        CardInstance temp;

        if (transform.TryGetComponent<CardInstance>(out temp))
        {
            LoadCard(temp.card);
        }
    }

    public void LoadCard(CardInfo i_card)
    {
        if (i_card)
        {
            cardArt.sprite = i_card.art;
            cardCostText.text = i_card.cardCost.ToString();
            cardText.text = i_card.cardText;
            cardValueText.text = i_card.cardValue.ToString();
            cardTypeText.text = i_card.cardType;
            cardNameText.text = i_card.cardName;
            card = i_card;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
