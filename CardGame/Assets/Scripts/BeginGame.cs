using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BeginGame : NetworkBehaviour
{
    private bool check = false;

    // Update is called once per frame
    void Update()
    {
        if(check && NetworkServer.connections.Count >= 2)
        {
            
        }
    }
}
