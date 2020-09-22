using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Card" ,menuName = "Card")]
public class CardInfo : ScriptableObject
{
    public string cardName;
    public string art;
    [TextArea] public string cardText;
    public string cardType;
    public string starValue;
    public string cardSymbol;
    public int cardDamage;
    public bool m_cantBeDodged = false;
    public bool m_cantBeBlocked = false;
    public bool m_cantBeCountered = false;
}
