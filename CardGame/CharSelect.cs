using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{
    [SerializeField] private Image m_bHSelector;
    [SerializeField] private Image m_lMSelector;
    [SerializeField] private List<CardInfo> m_LMDeck;
    [SerializeField] private List<CardInfo> m_BHDeck;
    public List<CardInfo> m_Deck = null;

    public void SelectBH()
    {
        m_bHSelector.gameObject.SetActive(true);
        m_lMSelector.gameObject.SetActive(false);
    }

    public void SelectLM()
    {
        m_lMSelector.gameObject.SetActive(true);
        m_bHSelector.gameObject.SetActive(false);
    }
}
