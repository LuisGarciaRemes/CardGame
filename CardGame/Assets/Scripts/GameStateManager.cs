﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private CardInstance m_lastPlayedCard;
    private PlayerManagerScript m_attackingPlayer;
    private PlayerManagerScript m_defendingPlayer;
    public static GameStateManager m_instance;
    private bool m_isGameOver = true;
    public enum RoundPhase { RoundStart, RoundMid, RoundEnd };
    private RoundPhase m_currPhase = RoundPhase.RoundStart;
    private bool m_hasChosenFirstAttacker = false;
    [SerializeField] private MouseControls m_controls;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetAttackingPlayer(PlayerManagerScript i_attacker)
    {
        m_attackingPlayer = i_attacker;
    }

    public void SetDefendingPlayer(PlayerManagerScript i_defender)
    {
        m_defendingPlayer = i_defender;
    }

    public void SetLastPlayedCard(CardInstance i_card)
    {
        m_lastPlayedCard = i_card;
    }

    public bool IsGameOver()
    {
        return m_isGameOver;
    }

    public void SetGameOver(bool i_bool)
    {
        m_isGameOver = i_bool;
    }

    public PlayerManagerScript GetAttackingPlayer()
    {
        return m_attackingPlayer;
    }

    public PlayerManagerScript GetDefendingPlayer()
    {
        return m_defendingPlayer;
    }

    public RoundPhase GetCurrPhase()
    {
        return m_currPhase;
    }

    public void SetCurrPhase(RoundPhase i_phase)
    {
        m_currPhase = i_phase;
    }

    public bool FirstPlayerChosen()
    {
        return m_hasChosenFirstAttacker;
    }

    public void SetFirstPlayerChosen(bool i_bool)
    {
        m_hasChosenFirstAttacker = i_bool;
    }

    public void EnablePassButton()
    {
        m_controls.EnablePassButton();
    }

    public void DisablePassButton()
    {
        m_controls.DisablePassButton();
    }

    public CardInstance GetLastPlayedCard()
    {
        return m_lastPlayedCard;
    }

}