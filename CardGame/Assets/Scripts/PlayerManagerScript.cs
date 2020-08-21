using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerManagerScript : NetworkBehaviour
{
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject DiscardPrefab;
    internal Deck m_myDeck;
    internal DiscardPile m_myDiscard;
    internal bool m_canDiscard = true;
    internal bool m_canDraw = true;

    private CardUI m_highlightedCard;
    internal GameObject m_heldCard = null;
    internal GameObject m_myHand;
    internal GameObject m_oppHand;
    internal GameObject m_myArea;
    internal GameObject m_oppArea;

    public override void OnStartClient()
    {
        Cursor.lockState = CursorLockMode.Confined;
        m_highlightedCard = GameObject.Find("HighlightedPrefab").GetComponent<CardUI>();
        m_myHand = GameObject.Find("MyHand");
        m_oppHand = GameObject.Find("OppHand");
        m_myArea = GameObject.Find("MyArea");
        m_oppArea = GameObject.Find("OppArea");

        GameObject.Find("Controls").GetComponent<MouseControls>().AddCount();
    }
    [ClientRpc]
    public void RpcUnparentCard()
    {
        if (hasAuthority)
        {
            m_heldCard.transform.SetParent(m_heldCard.transform.parent.transform.parent, false);
        }
    }

    [Command]
    public void CmdAddDeck()
    {
        m_myDeck = Instantiate(DeckPrefab, GameObject.Find("Screen Space (World)").transform).GetComponent<Deck>();
        NetworkServer.Spawn(m_myDeck.gameObject, connectionToClient);
    }

    [Command]
    public void CmdAddDiscard()
    {
        m_myDiscard = Instantiate(DiscardPrefab, GameObject.Find("Screen Space (World)").transform).GetComponent<DiscardPile>();
        NetworkServer.Spawn(m_myDiscard.gameObject, connectionToClient);
    }


    public void RpcDisplayHighlightedCard(CardInfo info)
    {
        if (!m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(true);

            if (!m_highlightedCard.transform.GetChild(0).gameObject.activeSelf)
            {
                for (int i = 0; i < 7; i++)
                {
                    m_highlightedCard.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if (info != m_highlightedCard.card)
        {
            m_highlightedCard.LoadCard(info);
        }
    }

    public void RpcHideHighlightedCard()
    {
        if (m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    public void RpcSetHeldCard(GameObject i_card)
    {
        if (hasAuthority)
        {
            m_heldCard = i_card;
        }
    }

    [ClientRpc]
    public void RpcSetPos(Vector2 pos)
    {
        if (hasAuthority && m_heldCard != null)
        {
            m_heldCard.transform.localPosition = pos;
        }
    }

    [ClientRpc]
    public void RpcSetInPlay()
    {
        m_heldCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (hasAuthority)
        {
            m_heldCard.transform.SetParent(m_myArea.transform, false);
        }
        else
        {
            m_heldCard.transform.SetParent(m_oppArea.transform, false);
        }

        m_heldCard = null;
    }

    [ClientRpc]
    public void RpcSetInHand()
    {
        m_heldCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (hasAuthority)
        {
            m_heldCard.transform.SetParent(m_myHand.transform, false);
        }
        else
        {
            m_heldCard.transform.SetParent(m_oppHand.transform, false);
        }

        m_heldCard = null;
    }

    [ClientRpc]
    public void RpcSetDeckPos()
    {
        if(hasAuthority)
        {
            m_myDeck.transform.SetParent(GameObject.Find("Screen Space (World)").transform);
            m_myDeck.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(335.0f, -150.0f, 0.0f);
        }
        else
        {
            m_myDeck.transform.SetParent(GameObject.Find("Screen Space (World)").transform);
            m_myDeck.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-335.0f, 150.0f, 0.0f);
        }
    }
}

