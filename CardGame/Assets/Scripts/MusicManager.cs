using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager m_instance;
    private AudioSource source;
    [SerializeField] private AudioClip m_bellRing;
    [SerializeField] private AudioClip m_click;
    [SerializeField] private AudioClip m_fight;
    [SerializeField] private AudioClip m_punchMiss;
    [SerializeField] private AudioClip m_punchBlocked;
    [SerializeField] private AudioClip m_clickError;
    [SerializeField] private AudioClip[] m_punchHit;
    [SerializeField] private AudioClip m_healthGain;
    [SerializeField] private AudioClip m_spentStar;
    [SerializeField] private AudioClip m_gainStar;
    [SerializeField] private AudioClip m_loseStar;
    [SerializeField] private AudioClip m_drawCard;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }

    public void PlayBell()
    {
        source.PlayOneShot(m_bellRing, 0.5f);
    }

    public void PlayClick()
    {
        source.PlayOneShot(m_click, 0.5f);
    }

    public void PlayFight()
    {
        source.PlayOneShot(m_fight, 0.5f);
    }

    public void PlayPunchHit()
    {
        source.PlayOneShot(m_punchHit[Random.Range(0, m_punchHit.Length)], 0.5f);
    }

    public void PlayPunchMiss()
    {
        source.PlayOneShot(m_punchMiss, 0.5f);
    }

    public void PlayPunchBlocked()
    {
        source.PlayOneShot(m_punchBlocked,0.5f);
    }

    public void PlayClickError()
    {
        source.PlayOneShot(m_clickError, 0.5f);
    }

    public void PlayHealthGain()
    {
        source.PlayOneShot(m_healthGain, 0.5f);
    }

    public void PlaySpentStar()
    {
        source.PlayOneShot(m_spentStar, 0.5f);
    }

    public void PlayGainStar()
    {
        source.PlayOneShot(m_gainStar, 0.25f);
    }

    public void PlayLoseStar()
    {
        source.PlayOneShot(m_loseStar, 0.5f);
    }

    public void PlayDrawCard()
    {
        source.PlayOneShot(m_drawCard, 0.5f);
    }
}
