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
    public Deck m_oppDeck;
    public DiscardPile m_oppDiscard;
    public bool m_canDiscard = true;
    public bool m_canDraw = false;

    private CardUI m_highlightedCard;
    public GameObject m_myHeldCard = null;
    public GameObject m_myHand;
    public GameObject m_oppHand;
    public GameObject m_myArea;
    public GameObject m_oppArea;

    public override void OnStartClient()
    {
        if (hasAuthority)
        {
            CmdCreateDeck();
        }
    }

    private void Start()
    {
        MouseControls.playersInGame++;
    }

    [Command]
    public void CmdSpawnCard(Transform i_trans, CardInfo i_info)
    {
        GameObject temp = Instantiate(CardPrefab, i_trans);
        NetworkServer.Spawn(temp, connectionToClient);
        temp.GetComponent<CardInstance>().LoadCardInfo(i_info);
        temp.GetComponent<CardInstance>().currState = CardInstance.CardState.Selected;
        temp.GetComponent<CardUI>().LoadCard(i_info);
        RpcSetHeldCard(temp);
    }

    public void DisplayHighlightedCard(CardInfo info)
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

    public void HideHighlightedCard()
    {
        if (m_highlightedCard.gameObject.activeSelf)
        {
            m_highlightedCard.gameObject.SetActive(false);
        }
    }

    [Command]
    public void CmdSetHeldCard(GameObject i_card)
    {
        RpcSetHeldCard(i_card);
    }

    [ClientRpc]
    public void RpcSetHeldCard(GameObject i_card)
    {
        if (hasAuthority)
        {
            i_card.transform.SetParent(m_myHand.transform.parent, true);
        }
        else
        {
            i_card.transform.SetParent(m_myHand.transform, true);
            i_card.GetComponent<CardInstance>().FlipCard(true);
        }

        i_card.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        i_card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        i_card.transform.localRotation = new Quaternion();
        m_myHeldCard = i_card;
    }

    public void SetHeldCardPos(Vector2 pos)
    {
        if (m_myHeldCard != null)
        {
            m_myHeldCard.transform.position = pos;
        }
    }

    [Command]
    public void CmdSetInPlay()
    {
        RpcSetInPlay();
    }

    [ClientRpc]
    public void RpcSetInPlay()
    {
        if(hasAuthority)
        {
            m_myHeldCard.GetComponent<CardInstance>().FlipCard(false);
        }
        m_myHeldCard.transform.SetParent(m_myArea.transform, true);
        m_myHeldCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        m_myHeldCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        m_myHeldCard.transform.localRotation = new Quaternion();
        m_myHeldCard = null;

    }

    [Command]
    public void CmdSetInHand()
    {
        RpcSetInHand();
    }

    [ClientRpc]
    public void RpcSetInHand()
    {
        if (m_myHeldCard)
        {
            m_myHeldCard.transform.SetParent(m_myHand.transform, false);
            m_myHeldCard.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            m_myHeldCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myHeldCard.transform.localRotation = new Quaternion();
            m_myHeldCard = null;
        }
    }


    [Command]
    public void CmdCreateDeck()
    {
        GameObject overlay = GameObject.Find("Screen Space (World)");
        GameObject temp = Instantiate(DeckPrefab, overlay.transform);
        NetworkServer.Spawn(temp,connectionToClient);
        m_myDeck = temp.GetComponent<Deck>();
        temp = Instantiate(DiscardPrefab, overlay.transform);
        NetworkServer.Spawn(temp, connectionToClient);
        m_myDiscard = temp.GetComponent<DiscardPile>();
        m_myHand = GameObject.Find("MyHand");
        m_oppHand = GameObject.Find("OppHand");
        m_myArea = GameObject.Find("MyArea");
        m_oppArea = GameObject.Find("OppArea");
        RpcSetMyPlayerReferences(m_myDeck.gameObject,m_myDiscard.gameObject);
    }

    [ClientRpc]
    public void RpcSetMyPlayerReferences(GameObject myDeck, GameObject myDiscard)
    {
        if (hasAuthority)
        {
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_highlightedCard = GameObject.Find("HighlightedPrefab").GetComponent<CardUI>();
            m_myDeck = myDeck.GetComponent<Deck>();
            myDeck.name = "MyDeck";
            m_myDiscard = myDiscard.GetComponent<DiscardPile>();
            myDiscard.name = "MyDiscard";
            m_myHand = GameObject.Find("MyHand");
            m_oppHand = GameObject.Find("OppHand");
            m_myArea = GameObject.Find("MyArea");
            m_oppArea = GameObject.Find("OppArea");
            m_myDeck.gameObject.transform.SetParent(overlay.transform, true);
            m_myDiscard.gameObject.transform.SetParent(overlay.transform, true);
            m_myDeck.gameObject.transform.localPosition = new Vector3(335.0f, -150.0f, 0.0f);
            m_myDiscard.gameObject.transform.localPosition = new Vector3(-335.0f, -150.0f, 0.0f);
            m_myDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.gameObject.name = "MyPlayer";
            m_myDeck.SwitchCardBack();
            GameObject.Find("Controls").GetComponent<MouseControls>().player = this;
        }
    }

    [Command]
    public void CmdSetOpposingReferences()
    {
        RpcSetOppPlayerReferences();
    }

    [ClientRpc]
    public void RpcSetOppPlayerReferences()
    {
        if (hasAuthority)
        {
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_oppDeck = GameObject.Find("OppDeck(Clone)").GetComponent<Deck>();
            m_oppDiscard = GameObject.Find("OppDiscard(Clone)").GetComponent<DiscardPile>();
            m_oppDeck.gameObject.transform.SetParent(overlay.transform, true);
            m_oppDiscard.gameObject.transform.SetParent(overlay.transform, true);
            m_oppDeck.gameObject.transform.localPosition = new Vector3(-335.0f, 150.0f, 0.0f);
            m_oppDiscard.gameObject.transform.localPosition = new Vector3(335.0f, 150.0f, 0.0f);
            m_oppDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_oppDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            PlayerManagerScript opp = GameObject.Find("OppPlayer(Clone)").GetComponent<PlayerManagerScript>();
            opp.m_myDeck = m_oppDeck;
            opp.m_myDiscard = m_oppDiscard;
            opp.m_oppDeck = m_myDeck;
            opp.m_oppDiscard = m_myDiscard;
            opp.m_myHand = m_oppHand;
            opp.m_myArea = m_oppArea;
            opp.m_oppArea = m_myArea;
            opp.m_oppHand = m_myHand;
        }
    }

}

