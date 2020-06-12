﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseControls : MonoBehaviour
{
    internal GameZone currZone = GameZone.Null;
    public Vector2 PlayZonePos;
    public Vector2 PlayZoneDim;
    public Vector2 DiscardZonePos;
    public Vector2 DiscardZoneDim;


    public enum GameZone { Hand, Discard, Deck, Play, Null };

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

       // Debug.Log(pointerData.position.ToString());
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        ClickableInterface c = null;

        if (pointerData.position.x <= (PlayZonePos.x + PlayZoneDim.x / 2) && pointerData.position.x >= (PlayZonePos.x - PlayZoneDim.x / 2) && pointerData.position.y <= (PlayZonePos.y + PlayZoneDim.y / 2) && pointerData.position.y >= (PlayZonePos.y - PlayZoneDim.y / 2))
        {
            currZone = GameZone.Play;
        }
        else if (pointerData.position.x <= (DiscardZonePos.x + DiscardZoneDim.x / 2) && pointerData.position.x >= (DiscardZonePos.x - DiscardZoneDim.x / 2) && pointerData.position.y <= (DiscardZonePos.y + DiscardZoneDim.y / 2) && pointerData.position.y >= (DiscardZonePos.y - DiscardZoneDim.y / 2))
        {
            currZone = GameZone.Discard;
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
                GameStateManager.instance.HideHighlightedCard();
            }          
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (c != null)
            {
                c.OnClick(currZone);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(GameStateManager.instance.HeldCard != null)
            {
                c.OnRelease(currZone);
            }
        }

        if(GameStateManager.instance.HeldCard != null)
        {
            GameStateManager.instance.HeldCard.transform.position = pointerData.position;                      
        }
    }
}
