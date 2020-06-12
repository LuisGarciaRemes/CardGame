using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager m_instance;
    [SerializeField] private CardUI HighlightedCard;
    internal GameObject HeldCard = null;
    public GameObject myHand;
    public GameObject myPlayArea;
    internal List<CardInfo> DiscardPile;
    [SerializeField] private CardUI TopDiscard;
    internal bool canDiscard = true;


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

    private void Start()
    {
        DiscardPile = new List<CardInfo>();
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

    private void DisplayTopDiscardCard(CardInfo info)
    {
        if (!TopDiscard.gameObject.activeSelf)
        {
            TopDiscard.gameObject.SetActive(true);
        }

        if (info != TopDiscard.card)
        {
            TopDiscard.LoadCard(info);
        }
    }

    public void DiscardCard(CardInfo info)
    {
        DiscardPile.Add(info);
        DisplayTopDiscardCard(info);
    }

    public void ReturnCardToHand()
    {

    }
}
