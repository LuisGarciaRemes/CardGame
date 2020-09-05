using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{
    [SerializeField] private Image m_bHSelector;
    [SerializeField] private Image m_lMSelector;
    private int m_deckID = -1;
    [SerializeField] private Toggle check;

    public void SelectBH()
    {
        m_bHSelector.gameObject.SetActive(true);
        m_lMSelector.gameObject.SetActive(false);
        check.isOn = true;
        m_deckID = 1;
        MusicManager.m_instance.PlayClick();
    }

    public void SelectLM()
    {
        m_lMSelector.gameObject.SetActive(true);
        m_bHSelector.gameObject.SetActive(false);
        check.isOn = true;
        m_deckID = 0;
        MusicManager.m_instance.PlayClick();
    }

    public int GetDeckID()
    {
        return m_deckID;
    }
}
