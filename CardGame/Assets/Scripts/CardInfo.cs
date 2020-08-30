using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Card" ,menuName = "Card")]
public class CardInfo : ScriptableObject
{
    public string cardName;
    public string art;
    public string cardText;
    public string cardType;
    public string starValue;
    public string cardSymbol;
    public int cardDamage;
}
