using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    List<CardInfo> DeckList;
    [SerializeField] private Text Amount;
    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject CardBack;
    private static System.Random rng;

    private void Start()
    {
        DeckList = new List<CardInfo>();
        System.Random rng = new System.Random();
    }

    public void DrawTopCard()
    {
        GameObject temp = Instantiate(CardPrefab);
        temp.GetComponent<CardInstance>().LoadCardInfo(DeckList[DeckList.Count-1]);
        DeckList.RemoveAt(DeckList.Count-1);
        Amount.text = DeckList.Count.ToString();

        if(DeckList.Count == 0)
        {
            CardBack.SetActive(false);
        }
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
