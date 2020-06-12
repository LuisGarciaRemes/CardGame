using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ClickableInterface
{
    void OnClick(MouseControls.GameZone i_zone);
    void OnHighlighted();
    void OnRelease(MouseControls.GameZone i_zone);
}
