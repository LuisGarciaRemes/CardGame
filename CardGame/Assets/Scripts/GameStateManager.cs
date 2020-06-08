using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager m_instance;
    [SerializeField] private CardUI HighlightedCard;
    internal GameObject HeldCard = null;

    public static GameStateManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("GameStateManager").AddComponent<GameStateManager>();
            }
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
            Destroy(gameObject);
    }

    public void DisplayHighlightedCard(CardInfo info)
    {
        if (!HighlightedCard.gameObject.activeSelf)
        {
            HighlightedCard.gameObject.SetActive(true);
        }

        if (info != HighlightedCard.card)
        {
            HighlightedCard.LoadCard(info);
        }
    }

    public void HideHighlightedCard()
    {
        if (HighlightedCard.gameObject.activeSelf)
        {
            HighlightedCard.gameObject.SetActive(false);
        }
    }

    public void SetHeldCard(GameObject i_card)
    {
        HeldCard = i_card;
    }
}
