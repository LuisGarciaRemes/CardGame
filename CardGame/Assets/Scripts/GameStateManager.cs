using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager m_instance;
    [SerializeField] private CardUI HighlightedCard;
    internal GameObject HeldCard = null;
    public GameObject myHand;
    public GameObject myPlayArea;
    public DiscardPile MyDiscard;
    public DiscardPile OpponentDiscard;
    public Deck MyDeck;
    internal bool canDiscard = true;
    internal bool canDraw = true;

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
        Cursor.lockState = CursorLockMode.Confined;
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
