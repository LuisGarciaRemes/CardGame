using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManagerScript : NetworkBehaviour
{
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject DiscardPrefab;
    internal Deck m_myDeck;
    internal DiscardPile m_myDiscard;
    internal bool m_canDiscard = true;
    internal bool m_canDraw = true;

    public override void OnStartClient()
    {
        CmdAddDeck();
        CmdAddDiscard();
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
}

