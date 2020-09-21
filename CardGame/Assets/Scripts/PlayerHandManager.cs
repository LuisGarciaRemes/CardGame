using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandManager : MonoBehaviour
{   
    [SerializeField] GameObject m_myHand;

    public void SetPlayableCards(PlayerManagerScript i_player)
    {
        foreach (CardInstance card in m_myHand.GetComponentsInChildren<CardInstance>())
        {
            if (GameStateManager.m_instance.GetAttackingPlayer() == i_player)
            {
                if(card.GetColor() == CardInstance.CardColor.Blue || card.GetColor() == CardInstance.CardColor.Yellow)
                {
                    card.SetCanPlay(true);
                    card.transform.GetComponent<CardUI>().SetOutlineColor(true);
                }
            }
            else
            {
                //To do. Check what cards can be played
            }
        }
    }

    public void SetAllUnplayable()
    {
        foreach(CardInstance card in m_myHand.GetComponentsInChildren<CardInstance>())
        {
            card.SetCanPlay(false);
            card.transform.GetComponent<CardUI>().SetOutlineColor(false);
        }
    }
}
