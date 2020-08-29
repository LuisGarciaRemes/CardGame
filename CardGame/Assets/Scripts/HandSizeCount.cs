using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSizeCount : MonoBehaviour
{
    public GameObject oppHand;
    public Text amount;

    private void Update()
    {
        if(amount.text != oppHand.transform.childCount.ToString())
        {
            amount.text = oppHand.transform.childCount.ToString();
        }
    }
}
