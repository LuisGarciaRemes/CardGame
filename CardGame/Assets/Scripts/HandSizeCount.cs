﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSizeCount : MonoBehaviour
{
    public GameObject oppHand;

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(oppHand.GetComponent<RectTransform>());
    }
}
