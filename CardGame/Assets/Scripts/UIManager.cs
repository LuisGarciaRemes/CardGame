using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{
    private static UIManager m_instance;
    [SerializeField] private CardUI m_highlightedCard;
    internal GameObject m_heldCard = null;
    public GameObject m_myHand;
    public GameObject m_oppHand;
    public GameObject m_myArea;
    public GameObject m_oppArea;

    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("UIManager").AddComponent<UIManager>();
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
        if (!m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(true);
        }

        if (info != m_highlightedCard.card)
        {
            m_highlightedCard.LoadCard(info);
        }
    }

    public void HideHighlightedCard()
    {
        if (m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(false);
        }
    }

    public void SetHeldCard(GameObject i_card)
    {
        m_heldCard = i_card;
    }
}
