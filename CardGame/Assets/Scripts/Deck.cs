using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Deck : NetworkBehaviour
{
    [SerializeField] private List<CardInfo> DeckList;
    [SerializeField] private Text Amount;
    [SerializeField] public GameObject CardBack;
    GameObject ScreenSpace;
    private static System.Random rng = new System.Random();

    private void Start()
    {
        ScreenSpace = GameObject.Find("Screen Overlay (Hand)");
        Amount.text = DeckList.Count.ToString();

        if (DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }
    }

    public void DrawTopCard()
    {
        PlayerManagerScript player = NetworkClient.connection.identity.GetComponent<PlayerManagerScript>();
        if (DeckList.Count > 0)
        {
            CardInfo tempinfo = DeckList[DeckList.Count - 1];
            player.CmdSpawnCard(ScreenSpace.transform,tempinfo);
            DeckList.RemoveAt(DeckList.Count - 1);
            CmdRemoveCardFromDeck();
        }

        if(DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }

        Amount.text = DeckList.Count.ToString();
    }

    [Command]
    public void CmdRemoveCardFromDeck()
    {
        RpcRemoveCardFromDeck();
    }

    [ClientRpc]
    public void RpcRemoveCardFromDeck()
    {
        if (!hasAuthority)
        {
            DeckList.RemoveAt(DeckList.Count - 1);
            if (DeckList.Count <= 0)
            {
                CardBack.SetActive(false);
            }
            Amount.text = DeckList.Count.ToString();
        }
    }

    public void SetDeckList(List<CardInfo> i_DeckList)
    {
        DeckList = new List<CardInfo>(i_DeckList);

        if (DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }
        else
        {
            CardBack.SetActive(true);
        }

        Amount.text = DeckList.Count.ToString();
    }

    public List<CardInfo> GetDeckList()
    {
        return DeckList;
    }

    public void Shuffle()
    {
        int n = DeckList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardInfo value = DeckList[k];
            DeckList[k] = DeckList[n];
            DeckList[n] = value;
        }
    }
}
