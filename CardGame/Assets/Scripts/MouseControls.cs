using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class MouseControls : MonoBehaviour
{
    internal GameZone currZone = GameZone.Null;
    public Vector2 PlayZonePos;
    public Vector2 PlayZoneDim;
    public Vector2 DiscardZonePos;
    public Vector2 DiscardZoneDim;
    public Vector2 DeckZonePos;
    public Vector2 DeckZoneDim;
    public Vector2 OppDiscardZonePos;
    public Vector2 OppDiscardZoneDim;
    public PlayerManagerScript player;

    public enum GameZone { Hand, MyDiscard, OppDiscard, Deck, Play, Null };

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            //Debug.Log(pointerData.position.ToString());
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
                    player.m_myDiscard.DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    player.m_myDiscard.DisplayPrevious();
                }
                currZone = GameZone.MyDiscard;
            }
            else if (pointerData.position.x <= (OppDiscardZonePos.x + OppDiscardZoneDim.x / 2) && pointerData.position.x >= (OppDiscardZonePos.x - OppDiscardZoneDim.x / 2) && pointerData.position.y <= (OppDiscardZonePos.y + OppDiscardZoneDim.y / 2) && pointerData.position.y >= (OppDiscardZonePos.y - OppDiscardZoneDim.y / 2))
            {
                /*
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    UIManager.instance.m_discardPiles[1].DisplayNext();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    UIManager.instance.m_discardPiles[1].DisplayPrevious();
                }
               
                currZone = GameZone.OppDiscard;
                */
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
                    player.RpcHideHighlightedCard();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (c != null)
                {
                    c.OnClick(currZone);
                }
                else if (currZone == GameZone.Deck)
                {
                    player.m_myDeck.DrawTopCard();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (player.m_myHeldCard != null)
                {
                    if (c == null)
                    {
                        c = player.m_myHeldCard.GetComponent<ClickableInterface>();
                    }

                    c.OnRelease(currZone);
                }
            }
            
            player.CmdSetHeldCardPos(pointerData.position);
        }
    }
}
