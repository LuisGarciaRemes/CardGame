using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DiscardPile : NetworkBehaviour
{
    [SerializeField] private List<CardInfo> PileList;
    [SerializeField] private Text Amount;
    [SerializeField] private CardUI TopCard;
    private int DisplayIndex = 0;
    [SerializeField] private Color m_unselected;
    [SerializeField] private Color m_selected;
    [SerializeField] private Image m_outline;

    private void Start()
    {
        PileList = new List<CardInfo>();
        TopCard.GetComponent<CardInstance>().SetCardState(CardInstance.CardState.InDiscard);
    }

    private void DisplayCard()
    {
        if (!TopCard.gameObject.activeSelf)
        {
            TopCard.gameObject.SetActive(true);
        }

        if (TopCard.card == null || PileList[DisplayIndex] != TopCard.card)
        {
            TopCard.LoadCard(PileList[DisplayIndex]);
            TopCard.transform.GetComponent<CardInstance>().LoadCardInfo(PileList[DisplayIndex]);
        }

        Amount.text = PileList.Count.ToString();
    }

    public void DisplayNext()
    {
        if (PileList.Count <= 0)
        {
            return;
        }

        if (DisplayIndex < PileList.Count-1)
        {
            DisplayIndex++;
        }
        else
        {
            DisplayIndex = 0;
        }
        DisplayCard();
    }

    public void DisplayPrevious()
    {
        if (PileList.Count <= 0)
        {
            return;
        }

        if (DisplayIndex <= 0)
        {
            DisplayIndex = PileList.Count-1;
        }
        else
        {
            DisplayIndex--;
        }
        DisplayCard();
    }

    [Command]
    public void CmdDiscardCard(CardInfo info , GameObject obj)
    {
        RpcDiscardCard(info);
        NetworkServer.Destroy(obj);
    }

    [ClientRpc]
    public void RpcDiscardCard(CardInfo info)
    {
        PileList.Add(info);
        DisplayIndex = PileList.Count - 1;
        DisplayCard();
    }

    public void SetOutline(bool i_val)
    {
        if (i_val)
        {
            m_outline.color = m_selected;
        }
        else
        {
            m_outline.color = m_unselected;
        }
    }

    public void ReturnCardToHand()
    {

    }
}
