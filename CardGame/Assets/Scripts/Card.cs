using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   [CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite art;
    public string cardText;
    public string cardType;
    public int cardCost;
    public int cardValue;
}
