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
                CardInstance lastPlayedCard = GameStateManager.m_instance.GetLastPlayedCard();

                if (lastPlayedCard.GetColor() == CardInstance.CardColor.Blue && card.GetColor() == CardInstance.CardColor.Red)
                {
                    if(card.GetCard().cardName.Contains("Counter") || lastPlayedCard.GetSide() == card.GetSide() || lastPlayedCard.GetHeight() == card.GetHeight() || (lastPlayedCard.GetSide() == CardInstance.CardSide.Straight && (card.GetSide() == CardInstance.CardSide.Left || card.GetSide() == CardInstance.CardSide.Right)))
                    {
                        if ((lastPlayedCard.IsUnblockable() && card.GetCard().cardName.Contains("Block")) || (lastPlayedCard.IsUndodgeable() && card.GetCard().cardName.Contains("Dodge")) || (lastPlayedCard.IsUncounterable() && card.GetCard().cardName.Contains("Counter")))
                        {
                            
                        }
                        else 
                        {
                            card.SetCanPlay(true);
                            card.transform.GetComponent<CardUI>().SetOutlineColor(true);
                        }
                    }                  
                }
                else if (lastPlayedCard.GetColor() == CardInstance.CardColor.Yellow && card.GetColor() == CardInstance.CardColor.Blue)
                {
                    if (lastPlayedCard.GetSide() == card.GetSide() || lastPlayedCard.GetHeight() == card.GetHeight() || (lastPlayedCard.GetSide() == CardInstance.CardSide.Straight && (card.GetSide() == CardInstance.CardSide.Left || card.GetSide() == CardInstance.CardSide.Right)) )
                    {
                        card.SetCanPlay(true);
                        card.transform.GetComponent<CardUI>().SetOutlineColor(true);
                    }
                }

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
