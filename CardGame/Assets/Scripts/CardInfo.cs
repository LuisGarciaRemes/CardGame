using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card" ,menuName = "Card")]
public class CardInfo : ScriptableObject
{
    public string cardName;
    public Sprite art;
    public string cardText;
    public string cardType;
    public int cardCost;
    public int cardValue;
}
