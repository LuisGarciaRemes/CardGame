using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Deck : NetworkBehaviour
{
    public List<CardInfo> DeckList;
    [SerializeField] private Text Amount;
    [SerializeField] GameObject CardBack;
    public Sprite altback;
    GameObject ScreenSpace;
    private static System.Random rng = new System.Random();

    private void Start()
    {
        ScreenSpace = GameObject.Find("Screen Overlay (Hand)");
        Shuffle();
        Amount.text = DeckList.Count.ToString();

        if (DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }
    }

    public void SwitchCardBack()
    {
        CardBack.GetComponent<Image>().sprite = altback;
    }

    public void DrawTopCard()
    {
        PlayerManagerScript player = NetworkClient.connection.identity.GetComponent<PlayerManagerScript>();
        if (DeckList.Count > 0)
        {
            CardInfo tempinfo = DeckList[DeckList.Count - 1];
            player.CmdSpawnCard(ScreenSpace.transform,tempinfo);
            DeckList.RemoveAt(DeckList.Count - 1);
        }

        if(DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }

        Amount.text = DeckList.Count.ToString();
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
