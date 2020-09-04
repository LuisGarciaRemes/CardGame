using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public CardInfo m_lastPlayedCard;
    public PlayerManagerScript m_attackingPlayer;
    public PlayerManagerScript m_defendingPlayer;
    public static GameStateManager m_instance;
    public bool isGameOver = true;
    public enum RoundPhase {RoundStart,RoundMid,RoundEnd };
    public RoundPhase m_currPhase = RoundPhase.RoundStart;
    public bool m_hasChosenFirstAttacker = false;

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

}
