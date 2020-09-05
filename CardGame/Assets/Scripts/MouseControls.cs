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
    private PlayerManagerScript player;
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
        if (playersInGame == 2 && player && GameStateManager.m_instance.IsGameOver())
        {
            player.CmdSetOpposingReferences();
            button.gameObject.SetActive(false);
            player.CmdPlayerIsReady();
            MusicManager.m_instance.PlayClick();
            GameStateManager.m_instance.SetGameOver(false);
        }
        else if(!GameStateManager.m_instance.IsGameOver() && player.GetHandCardCount() == 7)
        {
            button.gameObject.SetActive(false);
            MusicManager.m_instance.PlayFight();
            GameStateManager.m_instance.SetCurrPhase(GameStateManager.RoundPhase.RoundMid);

            if(player == GameStateManager.m_instance.GetAttackingPlayer())
            {
                player.SetCanPlayCards(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {

        int count = player.GetHandCardCount();

        if (playersInGame == 2 && player && !button.interactable)
        {
            button.interactable = true;
            button.GetComponentInChildren<Text>().text = "Ready Up";
        }
        else if(playersInGame == 2 && player && count >= 7 && GameStateManager.m_instance.GetCurrPhase() == GameStateManager.RoundPhase.RoundStart)
        {
            if (count > 7)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
            }
        }

        if (playersReady >= 2)
        {
            player.SetCanDraw(true);
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
                    player.GetDiscardPile().DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    player.GetDiscardPile().DisplayPrevious();
                }
                currZone = GameZone.MyDiscard;
            }
            else if (pointerData.position.x <= (OppDiscardZonePos.x + OppDiscardZoneDim.x / 2) && pointerData.position.x >= (OppDiscardZonePos.x - OppDiscardZoneDim.x / 2) && pointerData.position.y <= (OppDiscardZonePos.y + OppDiscardZoneDim.y / 2) && pointerData.position.y >= (OppDiscardZonePos.y - OppDiscardZoneDim.y / 2))
            {             
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    player.GetOppDiscardPile().DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    player.GetOppDiscardPile().DisplayPrevious();
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
                    player.HideHighlightedCard();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (c != null)
                {
                    c.OnClick(currZone);
                }
                else if (currZone == GameZone.Deck && player.CanDrawCards())
                {
                    player.GetMyDeck().DrawTopCard();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (player.IsHoldingACard())
                {
                    if (c == null)
                    {
                        c = player.GetHeldCard().GetComponent<ClickableInterface>();
                    }

                    c.OnRelease(currZone);
                }
            }

            if (player.IsHoldingACard())
            {
                player.GetHeldCard().transform.position = pointerData.position;
            }
        }
    }

    public void SetPlayer(PlayerManagerScript i_player)
    {
        player = i_player;
    }

    static public void AddPlayerInGame()
    {
        playersInGame++;
    }

    static public void AddPlayerReady()
    {
        playersReady++;
    }
}
