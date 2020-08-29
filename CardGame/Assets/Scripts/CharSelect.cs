using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{
    [SerializeField] private Image m_bHSelector;
    [SerializeField] private Image m_lMSelector;
    public int m_deckID = -1;
    public Toggle check;

    public void SelectBH()
    {
        m_bHSelector.gameObject.SetActive(true);
        m_lMSelector.gameObject.SetActive(false);
        check.isOn = true;
        m_deckID = 1;
    }

    public void SelectLM()
    {
        m_lMSelector.gameObject.SetActive(true);
        m_bHSelector.gameObject.SetActive(false);
        check.isOn = true;
        m_deckID = 0;
    }
}
