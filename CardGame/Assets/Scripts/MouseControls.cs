using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using UnityEngine.UI;

public class MouseControls : MonoBehaviour
{
    private GameZone currZone = GameZone.Null;
    [SerializeField] private Vector2 PlayZonePos;
    [SerializeField] private Vector2 PlayZoneDim;
    [SerializeField] private Vector2 DiscardZonePos;
    [SerializeField] private Vector2 DiscardZoneDim;
    [SerializeField] private Vector2 DeckZonePos;
    [SerializeField] private Vector2 DeckZoneDim;
    [SerializeField] private Vector2 OppDiscardZonePos;
    [SerializeField] private Vector2 OppDiscardZoneDim;
    private PlayerManagerScript m_player;
    [SerializeField] private Button button;
    private static int playersInGame = 0;
    private static int playersReady = 0;

    public enum GameZone { Hand, MyDiscard, OppDiscard, Deck, Play, Null };

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ReadyUp()
    {
        if (button.GetComponentInChildren<Text>().text == "Pass")
        {
            MusicManager.m_instance.PlayClick();
            button.gameObject.SetActive(false);
            m_player.CmdPassTheTurn();
        }
        else if (playersInGame == 2 && m_player && GameStateManager.m_instance.IsGameOver())
        {
            m_player.CmdSetOpposingReferences();
            button.gameObject.SetActive(false);
            m_player.CmdPlayerIsReady();
            MusicManager.m_instance.PlayClick();
            GameStateManager.m_instance.SetGameOver(false);
        }
        else if(!GameStateManager.m_instance.IsGameOver() && m_player.GetHandCardCount() == 7)
        {
            button.gameObject.SetActive(false);
            MusicManager.m_instance.PlayFight();
            MusicManager.m_instance.PlayBell();
            GameStateManager.m_instance.SetCurrPhase(GameStateManager.RoundPhase.RoundMid);

            if(m_player == GameStateManager.m_instance.GetAttackingPlayer())
            {
                m_player.SetCanPlayCards(true);
                m_player.CmdSetInitialMarkers();
                m_player.UpdatePlayable();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player)
        {

        int count = m_player.GetHandCardCount();

        if (playersInGame == 2 && m_player && !button.interactable)
        {
            button.interactable = true;
            button.GetComponentInChildren<Text>().text = "Ready Up";
        }
        else if(playersInGame == 2 && m_player && count >= 7 && GameStateManager.m_instance.GetCurrPhase() == GameStateManager.RoundPhase.RoundStart)
        {
            if (count > 7)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.GetComponentInChildren<Text>().text = "Ready Up";
                button.gameObject.SetActive(true);
            }
        }

        if (playersReady >= 2)
        {
            m_player.SetCanDraw(true);
            playersReady = 0;
        }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            ClickableInterface c = null;

            if (pointerData.position.x <= (PlayZonePos.x + PlayZoneDim.x / 2) && pointerData.position.x >= (PlayZonePos.x - PlayZoneDim.x / 2) && pointerData.position.y <= (PlayZonePos.y + PlayZoneDim.y / 2) && pointerData.position.y >= (PlayZonePos.y - PlayZoneDim.y / 2))
            {
                currZone = GameZone.Play;
            }
            else if (pointerData.position.x <= (DiscardZonePos.x + DiscardZoneDim.x / 2) && pointerData.position.x >= (DiscardZonePos.x - DiscardZoneDim.x / 2) && pointerData.position.y <= (DiscardZonePos.y + DiscardZoneDim.y / 2) && pointerData.position.y >= (DiscardZonePos.y - DiscardZoneDim.y / 2))
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    m_player.GetDiscardPile().DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    m_player.GetDiscardPile().DisplayPrevious();
                }
                currZone = GameZone.MyDiscard;
            }
            else if (pointerData.position.x <= (OppDiscardZonePos.x + OppDiscardZoneDim.x / 2) && pointerData.position.x >= (OppDiscardZonePos.x - OppDiscardZoneDim.x / 2) && pointerData.position.y <= (OppDiscardZonePos.y + OppDiscardZoneDim.y / 2) && pointerData.position.y >= (OppDiscardZonePos.y - OppDiscardZoneDim.y / 2))
            {             
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    m_player.GetOppDiscardPile().DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    m_player.GetOppDiscardPile().DisplayPrevious();
                }
               
                currZone = GameZone.OppDiscard;              
            }
            else if (pointerData.position.x <= (DeckZonePos.x + DeckZoneDim.x / 2) && pointerData.position.x >= (DeckZonePos.x - DeckZoneDim.x / 2) && pointerData.position.y <= (DeckZonePos.y + DeckZoneDim.y / 2) && pointerData.position.y >= (DeckZonePos.y - DeckZoneDim.y / 2))
            {
                currZone = GameZone.Deck;
            }
            else
            {
                currZone = GameZone.Null;
            }

            foreach (RaycastResult r in results)
            {
                c = r.gameObject.GetComponentInParent<ClickableInterface>();
                if (c != null)
                {
                    c.OnHighlighted();
                    break;
                }
                else
                {
                    m_player.HideHighlightedCard();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (c != null)
                {
                    c.OnClick(currZone);
                }
                else if (currZone == GameZone.Deck && m_player.CanDrawCards())
                {
                    m_player.GetMyDeck().DrawTopCard();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (m_player.IsHoldingACard())
                {
                    if (c == null)
                    {
                        c = m_player.GetHeldCard().GetComponent<ClickableInterface>();
                    }

                    c.OnRelease(currZone);
                }
            }

            if (m_player.IsHoldingACard())
            {
                m_player.GetHeldCard().transform.position = pointerData.position;
            }
        }
    }

    public void SetPlayer(PlayerManagerScript i_player)
    {
        m_player = i_player;
    }

    static public void AddPlayerInGame()
    {
        playersInGame++;
    }

    static public void AddPlayerReady()
    {
        playersReady++;
    }

    public void EnablePassButton()
    {
        button.GetComponentInChildren<Text>().text = "Pass";
        button.gameObject.SetActive(true);
    }

    public void DisablePassButton()
    {
        button.GetComponentInChildren<Text>().text = "Pass";
        button.gameObject.SetActive(false);
    }

    public void CallSpendStar()
    {
        m_player.SpendStar();
    }
}
