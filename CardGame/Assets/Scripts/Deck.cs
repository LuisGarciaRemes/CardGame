using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public List<CardInfo> DeckList;
    [SerializeField] private Text Amount;
    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject CardBack;
    [SerializeField] GameObject ScreenSpace;
    private static System.Random rng = new System.Random();

    private void Start()
    {
        Shuffle();
        Amount.text = DeckList.Count.ToString();

        if (DeckList.Count <= 0)
        {
            CardBack.SetActive(false);
        }
    }

    public void DrawTopCard()
    {
        if (DeckList.Count > 0)
        {
            GameObject temp = Instantiate(CardPrefab, ScreenSpace.transform);
            CardInfo tempinfo = DeckList[DeckList.Count - 1];
            temp.GetComponent<CardInstance>().LoadCardInfo(tempinfo);
            temp.GetComponent<CardInstance>().currState = CardInstance.CardState.Selected;
            temp.GetComponent<CardUI>().LoadCard(tempinfo);
            DeckList.RemoveAt(DeckList.Count - 1);
            GameStateManager.instance.SetHeldCard(temp);
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
