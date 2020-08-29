using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class PlayerManagerScript : NetworkBehaviour
{
    [SerializeField] private List<CardInfo> m_LMDeckList;
    [SerializeField] private List<CardInfo> m_BHDeckList;
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject DiscardPrefab;
    [SerializeField] private GameObject CardPrefab;
    public Deck m_myDeck;
    public DiscardPile m_myDiscard;
    public Deck m_oppDeck;
    public DiscardPile m_oppDiscard;
    internal bool m_canDiscard = true;
    internal bool m_canDraw = false;

    private CardUI m_highlightedCard;
    public GameObject m_myHeldCard = null;
    public GameObject m_myHand;
    public GameObject m_oppHand;
    public GameObject m_myArea;
    public GameObject m_oppArea;

    public int m_health = 20;
    public int m_maxDaze = 3;
    public int m_currDaze = 0;
    public int m_maxStar = 3;
    public int m_currStar = 0;

    public PlayerInfo m_myInfo;
    public int m_deckID = -1;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (hasAuthority)
        {
            CmdCreateDeckAndDiscard(m_deckID = GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().charSelect.GetComponent<CharSelect>().m_deckID);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (hasAuthority)
        {
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().charSelect.SetActive(true);
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
        RpcSetHeldCard(temp, i_info);
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
    public void CmdSetHeldCard(GameObject i_card, CardInfo i_info)
    {
        RpcSetHeldCard(i_card, i_info);
    }

    [ClientRpc]
    public void RpcSetHeldCard(GameObject i_card, CardInfo i_info)
    {
        i_card.GetComponent<CardInstance>().LoadCardInfo(i_info);
        i_card.GetComponent<CardInstance>().currState = CardInstance.CardState.Selected;
        i_card.GetComponent<CardUI>().LoadCard(i_info);

        if (hasAuthority)
        {
            i_card.transform.SetParent(m_myHand.transform.parent, false);
        }
        else
        {
            i_card.transform.SetParent(m_myHand.transform, false);
            i_card.GetComponent<CardInstance>().FlipCard(true);
        }
        i_card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        i_card.transform.localRotation = new Quaternion();
        m_myHeldCard = i_card;
    }

    [Command]
    public void CmdSetInPlay()
    {
        RpcSetInPlay();
    }

    [ClientRpc]
    public void RpcSetInPlay()
    {
        if (!hasAuthority)
        {
            m_myHeldCard.GetComponent<CardInstance>().FlipCard(false);
        }
        m_myHeldCard.GetComponent<CardInstance>().currState = CardInstance.CardState.InPlay;
        m_myHeldCard.transform.SetParent(m_myArea.transform, false);
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
            m_myHeldCard.GetComponent<CardInstance>().currState = CardInstance.CardState.InHand;
            m_myHeldCard.transform.SetParent(m_myHand.transform, false);
            m_myHeldCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myHeldCard.transform.localRotation = new Quaternion();
            m_myHeldCard = null;
        }
    }


    [Command]
    public void CmdCreateDeckAndDiscard(int i_deckID)
    {
        GameObject overlay = GameObject.Find("Screen Space (World)");
        GameObject temp = Instantiate(DeckPrefab, overlay.transform);
        NetworkServer.Spawn(temp,connectionToClient);
        m_myDeck = temp.GetComponent<Deck>();
        m_deckID = i_deckID;
        switch (m_deckID)
        {
            case 0:
                m_myDeck.SetDeckList(m_LMDeckList);
                break;
            case 1:
                m_myDeck.SetDeckList(m_BHDeckList);
                break;
            default:
                break;
        }
        temp = Instantiate(DiscardPrefab, overlay.transform);
        NetworkServer.Spawn(temp, connectionToClient);
        m_myDiscard = temp.GetComponent<DiscardPile>();
        m_myHand = GameObject.Find("MyHand");
        m_oppHand = GameObject.Find("OppHand");
        m_myArea = GameObject.Find("MyArea");
        m_oppArea = GameObject.Find("OppArea");
        m_myInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        RpcSetMyPlayerReferences(m_myDeck.gameObject,m_myDiscard.gameObject,m_deckID);
    }

    [ClientRpc]
    public void RpcSetMyPlayerReferences(GameObject myDeck, GameObject myDiscard , int i_deckID)
    {
        if (hasAuthority)
        {
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_highlightedCard = GameObject.Find("HighlightedPrefab").GetComponent<CardUI>();
            m_myDeck = myDeck.GetComponent<Deck>();
            m_deckID = i_deckID;
            switch (m_deckID)
            {
                case 0:
                    m_myDeck.SetDeckList(m_LMDeckList);
                    break;
                case 1:
                    m_myDeck.SetDeckList(m_BHDeckList);
                    break;
                default:
                    break;
            }
            m_myDeck.Shuffle();
            myDeck.name = "MyDeck";            
            m_myDiscard = myDiscard.GetComponent<DiscardPile>();
            myDiscard.name = "MyDiscard";
            m_myHand = GameObject.Find("MyHand");
            m_oppHand = GameObject.Find("OppHand");
            m_myArea = GameObject.Find("MyArea");
            m_oppArea = GameObject.Find("OppArea");
            m_myInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
            m_myDeck.gameObject.transform.SetParent(overlay.transform, false);
            m_myDiscard.gameObject.transform.SetParent(overlay.transform, false);
            m_myDeck.gameObject.transform.localPosition = new Vector3(335.0f, -150.0f, 0.0f);
            m_myDiscard.gameObject.transform.localPosition = new Vector3(-335.0f, -150.0f, 0.0f);
            m_myDeck.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_myDiscard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            this.gameObject.name = "MyPlayer";
            GameObject.Find("Controls").GetComponent<MouseControls>().player = this;
        }
    }

    [Command]
    public void CmdSetOpposingReferences()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int tempID = -1;

        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerManagerScript>() != this)
            {
                tempID = player.GetComponent<PlayerManagerScript>().m_deckID;
            }
        }

        RpcSetOppPlayerReferences(tempID);
    }

    [ClientRpc]
    public void RpcSetOppPlayerReferences(int i_deckID)
    {
        if (hasAuthority)
        {
            GameObject overlay = GameObject.Find("Screen Space (World)");
            m_oppDeck = GameObject.Find("OppDeck(Clone)").GetComponent<Deck>();
            m_oppDiscard = GameObject.Find("OppDiscard(Clone)").GetComponent<DiscardPile>();
            m_oppDeck.gameObject.transform.SetParent(overlay.transform, false);
            m_oppDiscard.gameObject.transform.SetParent(overlay.transform, false);
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
            opp.m_deckID = i_deckID;
            switch (opp.m_deckID)
            {
                case 0:
                    opp.m_myDeck.SetDeckList(m_LMDeckList);
                    break;
                case 1:
                    opp.m_myDeck.SetDeckList(m_BHDeckList);
                    break;
                default:
                    break;
            }
            opp.m_oppHand = m_myHand;
            opp.m_myInfo = GameObject.Find("OpponentInfo").GetComponent<PlayerInfo>();         
        }     
    }

    [Command]
    public void CmdPlayerIsReady()
    {
        RpcPlayerIsReady();
    }

    [ClientRpc]
    public void RpcPlayerIsReady()
    {
        MouseControls.playersReady++;
    }
}

