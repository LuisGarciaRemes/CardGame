using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Stats", menuName = "CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public int[] m_health = {0,0,0};
    public int m_dazeVal;
    public int m_starVal;
    public List<CardInfo> m_deckList;
    public Sprite m_pic;
}
