using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossManager : MonoBehaviour
{
    private float SecondsPerCheck = 5f;
    private float CheckTimer = 0;

    // Update is called once per frame
    void Update()
    {
    	if(CheckTimer > SecondsPerCheck)
    	{
    		CheckTimer = 0;
    		
    		//check if we own all nodes, if so we win
    		//Check if own no nodes, if so we lose
    		//foreach()

    	}
    	else
    	{
    		CheckTimer += Time.deltaTime;
    	}
    }
}
