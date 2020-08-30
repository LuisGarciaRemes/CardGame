using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Text m_maxStar;
    [SerializeField] private Text m_currStar;
    [SerializeField] private Text m_maxDaze;
    [SerializeField] private Text m_currDaze;
    [SerializeField] private Text m_name;
    [SerializeField] private Text m_health;
    [SerializeField] private Image m_char;

    public void UpdateStarMax(int i_value)
    {
        m_maxStar.text = "  / " + i_value.ToString();
    }

    public void UpdateStarCurr(int i_value)
    {
        m_currStar.text = i_value.ToString();
    }

    public void UpdateDazeMax(int i_value)
    {
        m_maxDaze.text = "  / " + i_value.ToString();
    }

    public void UpdateDazeCurr(int i_value)
    {
        m_currDaze.text = i_value.ToString();
    }

    public void UpdateName(string i_name)
    {
        m_name.text = i_name;
    }

    public void UpdateHealth(int i_value)
    {
        m_health.text = i_value.ToString();
    }

    public void UpdateCharacterStats(CharacterStats i_stats)
    {
        m_char.sprite = i_stats.m_pic;
        UpdateDazeMax(i_stats.m_dazeVal);
        UpdateStarMax(i_stats.m_starVal);
        UpdateHealth(i_stats.m_health[0]);
        UpdateName(i_stats.m_name);
    }
}
