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
    [SerializeField] private Image m_turnMarker;
    [SerializeField] private float m_flashTime = 1.0f;
    [SerializeField] GameObject m_attackingGlove;
    [SerializeField] GameObject m_blockingGloves;
    private float m_FlashTimer = 0.0f;
    private bool m_shouldFlash = false;
    private int m_frameCounter = 0;
    [SerializeField] private float m_punchSpeed = 1000.0f;
    private Vector3 m_orgGlovePos;
    private Vector3 m_orgCharPos;
    private bool m_shouldPunch = false;
    [SerializeField] private Color m_unselected;
    [SerializeField] private Color m_selected;
    private bool m_shouldDodge;

    private void Start()
    {
        m_orgGlovePos = m_attackingGlove.transform.position;
        m_orgCharPos = m_turnMarker.transform.position;
    }

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

    public void FlashRed()
    {
        m_shouldFlash = true;
    }

    public void PlayUnblockedPunchAnimation()
    {
        m_shouldPunch = true;
        m_attackingGlove.SetActive(true);
    }

    public void PlayBlockedPunchAnimation()
    {
        m_shouldPunch = true;
        m_attackingGlove.SetActive(true);
        m_blockingGloves.SetActive(true);
    }

    public void PlayDodgeAnimation()
    {
        m_shouldPunch = true;
        m_attackingGlove.SetActive(true);
        m_shouldDodge = true;
    }

    private void Update()
    {
        if (m_shouldFlash)
        {
            if (m_FlashTimer >= m_flashTime)
            {
                m_shouldFlash = false;
                m_char.color = Color.white;
                m_FlashTimer = 0.0f;
                m_frameCounter = 0;
            }
            else
            {
                m_FlashTimer += Time.deltaTime;

                if (m_frameCounter % 20 == 0)
                {
                    if (m_char.color == Color.white)
                    {
                        m_char.color = Color.red;
                    }
                    else
                    {
                        m_char.color = Color.white;
                    }
                }

                m_frameCounter++;
            }
        }

        if (m_shouldPunch)
        {
            m_attackingGlove.transform.position = Vector3.MoveTowards(m_attackingGlove.transform.position, m_orgCharPos, Time.deltaTime * m_punchSpeed);

            if ((m_orgCharPos - m_attackingGlove.transform.position).magnitude <= 50.0f)
            {
                m_shouldPunch = false;
                m_attackingGlove.SetActive(false);
                m_attackingGlove.transform.position = m_orgGlovePos;

                if (m_blockingGloves.activeSelf)
                {
                    MusicManager.m_instance.PlayPunchBlocked();
                    m_blockingGloves.SetActive(false);
                }
                else
                {
                    if (m_shouldDodge)
                    {
                        MusicManager.m_instance.PlayPunchMiss();
                        m_shouldDodge = false;
                        m_turnMarker.transform.position = m_orgCharPos;
                    }
                    else
                    {
                        MusicManager.m_instance.PlayPunchHit();
                        FlashRed();
                    }
                }
            }
        }

        if (m_shouldDodge)
        {
            m_turnMarker.transform.position = Vector3.MoveTowards(m_turnMarker.transform.position, m_orgCharPos - (m_turnMarker.transform.up * 100), Time.deltaTime * m_punchSpeed/2);
        }
    }

    public void SetSelected()
    {
        m_turnMarker.color = m_selected;
    }

    public void SetUnselected()
    {
        m_turnMarker.color = m_unselected;
    }
}
