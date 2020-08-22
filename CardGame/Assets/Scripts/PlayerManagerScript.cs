using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerManagerScript : NetworkBehaviour
{
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject DiscardPrefab;
    [SerializeField] private GameObject CardPrefab;
    public Deck m_myDeck;
    public DiscardPile m_myDiscard;
    internal bool m_canDiscard = true;
    internal bool m_canDraw = true;

    private CardUI m_highlightedCard;
    internal GameObject m_heldCard = null;
    public GameObject m_myHand;
    public GameObject m_oppHand;
    public GameObject m_myArea;
    public GameObject m_oppArea;

    public override void OnStartClient()
    {
        GameObject overlay = GameObject.Find("Screen Space (World)");
        if (hasAuthority)
        {
            m_highlightedCard = GameObject.Find("HighlightedPrefab").GetComponent<CardUI>();
            m_myHand = GameObject.Find("MyHand");
            m_oppHand = GameObject.Find("OppHand");
            m_myArea = GameObject.Find("MyArea");
            m_oppArea = GameObject.Find("OppArea");
            m_myDeck.gameObject.name = "MyDeck";
            m_myDiscard.gameObject.name = "MyDiscard";
            m_myDeck.gameObject.transform.SetParent(overlay.transform,true);
            m_myDiscard.gameObject.transform.SetParent(overlay.transform,true);
            m_myDeck.gameObject.transform.localPosition = new Vector3(335.0f,-150.0f,0.0f);
            m_myDiscard.gameObject.transform.localPosition = new Vector3(-335.0f, -150.0f, 0.0f);
            m_myDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.gameObject.name = "MyPlayer";
            m_myDeck.SwitchCardBack();
        }
        else
        {
            m_myHand = GameObject.Find("OppHand");
            m_oppHand = GameObject.Find("MyHand");
            m_myArea = GameObject.Find("OppArea");
            m_oppArea = GameObject.Find("MyArea");
            m_myDeck.gameObject.name = "OppDeck";
            m_myDiscard.gameObject.name = "OppDiscard";
            m_myDeck.gameObject.transform.SetParent(overlay.transform, true);
            m_myDiscard.gameObject.transform.SetParent(overlay.transform, true);
            m_myDeck.gameObject.transform.localPosition = new Vector3(-335.0f, 150.0f, 0.0f);
            m_myDiscard.gameObject.transform.localPosition = new Vector3(335.0f, 150.0f, 0.0f);
            m_myDeck.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
            m_myDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.gameObject.name = "OppPlayer";         
        }
    }

    [ClientRpc]
    public void RpcUnparentCard()
    {
        if (hasAuthority)
        {
            m_heldCard.transform.SetParent(m_heldCard.transform.parent.transform.parent, true);
        }
    }

    [Command]
    public void CmdSpawnCard(Transform i_trans, CardInfo i_info)
    {
        GameObject temp = Instantiate(CardPrefab,i_trans);
        temp.GetComponent<CardInstance>().LoadCardInfo(i_info);
        temp.GetComponent<CardInstance>().currState = CardInstance.CardState.Selected;
        temp.GetComponent<CardUI>().LoadCard(i_info);
        NetworkServer.Spawn(temp, connectionToClient);
        RpcSetHeldCard(temp);
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
        else
        {
            i_card.transform.SetParent(m_oppHand.transform,true);
        }
    }

    [ClientRpc]
    public void RpcSetPos(Vector2 pos)
    {
        if (hasAuthority && m_heldCard != null)
        {
            m_heldCard.transform.position = pos;
        }
    }

    [ClientRpc]
    public void RpcSetInPlay()
    {
        m_heldCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (hasAuthority)
        {
            m_heldCard.transform.SetParent(m_myArea.transform, true);
        }
        else
        {
            m_heldCard.transform.SetParent(m_oppArea.transform, true);
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
}

