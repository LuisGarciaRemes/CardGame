using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{
    [SerializeField] private Image m_bHSelector;
    [SerializeField] private Image m_lMSelector;
    private int m_deckID = -1;
    [SerializeField] private Toggle m_check;
    [SerializeField] private Color m_unselected;
    [SerializeField] private Color m_selected;

    public void SelectBH()
    {
        m_bHSelector.color = m_selected;
        m_lMSelector.color = m_unselected;
        m_check.isOn = true;
        m_deckID = 1;
        MusicManager.m_instance.PlayClick();
    }

    public void SelectLM()
    {
        m_lMSelector.color = m_selected;
        m_bHSelector.color = m_unselected;
        m_check.isOn = true;
        m_deckID = 0;
        MusicManager.m_instance.PlayClick();
    }

    public int GetDeckID()
    {
        return m_deckID;
    }
}
