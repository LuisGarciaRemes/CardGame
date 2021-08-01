using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Deck : NetworkBehaviour
{
    [SerializeField] private List<CardInfo> DeckList;
    [SerializeField] private Text Amount;
    [SerializeField] private GameObject CardBack;
    private GameObject ScreenSpace;
    private PlayerManagerScript player;
    private static System.Random rng = new System.Random();
    [SerializeField] private Color m_unselected;
    [SerializeField] private Color m_selected;
    [SerializeField] private Image m_outline;

    private void Start()
    {
        ScreenSpace = GameObject.Find("Screen Overlay (Hand)");
        Amount.text = DeckList.Count.ToString();

        if (DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }

        player = NetworkClient.connection.identity.GetComponent<PlayerManagerScript>();
    }

    public void DrawTopCard()
    {
        if (DeckList.Count > 0)
        {
            CardInfo tempinfo = DeckList[DeckList.Count - 1];
            player.CmdSpawnCard(ScreenSpace.transform,tempinfo);
            DeckList.RemoveAt(DeckList.Count - 1);
            CmdRemoveCardFromDeck();
            player.SetCardsDrawn(player.GetCardsDrawn()+1);

            if(player.GetCardsDrawn() == 7)
            {
                player.SetCanDraw(false);
                player.SetCardsDrawn(0);
            }

            MusicManager.m_instance.PlayDrawCard();
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
            if (DeckList.Count > 0)
            {
                DeckList.RemoveAt(DeckList.Count - 1);
            }

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

    public void SetOutline(bool i_val)
    {
        if(i_val)
        {
            m_outline.color = m_selected;
        }
        else
        {
            m_outline.color = m_unselected;
        }
    }
}
