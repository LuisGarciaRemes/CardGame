using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardUI : MonoBehaviour
{
    [SerializeField] Image cardArt;
    [SerializeField] Image starValue;
    [SerializeField] Image cardSymbol;
    [SerializeField] Text cardDamageText;
    [SerializeField] Text cardText;
    [SerializeField] Text cardTypeText;
    [SerializeField] Text cardNameText;
    [SerializeField] Image cardOutline;
    internal CardInfo card;
    [SerializeField] private Color m_unselected;
    [SerializeField] private Color m_selected;


    private void Start()
    {
        CardInstance temp;

        if (transform.TryGetComponent<CardInstance>(out temp))
        {
            LoadCard(temp.GetCard());
        }
    }

    public void LoadCard(CardInfo i_card)
    {
        if (i_card)
        {
            cardArt.sprite = Resources.Load<Sprite>(i_card.art);
            starValue.sprite = Resources.Load<Sprite>(i_card.starValue);
            cardSymbol.sprite = Resources.Load<Sprite>(i_card.cardSymbol);
            cardText.text = i_card.cardText;
            cardDamageText.text = i_card.cardDamage.ToString();

            if(cardDamageText.text == "0")
            {
                cardDamageText.text = "";
            }
            
            cardTypeText.text = i_card.cardType;
            cardNameText.text = i_card.cardName;
            card = i_card;
        }
        else
        {
            //gameObject.SetActive(false);
        }
    }

    public void SetOutlineColor(bool i_val)
    {
        if (i_val)
        {
            cardOutline.color = m_selected;
        }
        else
        {
            cardOutline.color = m_unselected;
        }
    }

}
