using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseControls : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        ClickableInterface c = null;

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
                c.OnClick();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(GameStateManager.instance.HeldCard != null)
            {
                c.OnRelease();
            }
        }

        if(GameStateManager.instance.HeldCard != null)
        {
            GameStateManager.instance.HeldCard.transform.position = pointerData.position;
        }
    }
}
