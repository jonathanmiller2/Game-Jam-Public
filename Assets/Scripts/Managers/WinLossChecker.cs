using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossChecker : MonoBehaviour
{
    private float SecondsPerCheck = 5f;
    private float CheckTimer = 0;

    // Update is called once per frame
    void Update()
    {
    	if(CheckTimer > SecondsPerCheck)
    	{
    		CheckTimer = 0;
    		
            bool WeOwnAllNodes = true;
            bool WeOwnNoNodes = true;

    		//check if we own all nodes, if so we win
    		//Check if own no nodes, if so we lose
    		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
            {
                if(obj.GetComponent<NodeScript>().GetOwner() == 1)
                {
                    WeOwnNoNodes = false;
                }
                else
                {
                    WeOwnAllNodes = false;
                }
            }

            if(WeOwnAllNodes)
            {
                GameObject.Find("Win Transition").GetComponent<Animator>().SetTrigger("Win");
            }
            else if(WeOwnNoNodes)
            {
                GameObject.Find("Lose Transition").GetComponent<Animator>().SetTrigger("Lose");   
            }
    	}
    	else
    	{
    		CheckTimer += Time.deltaTime;
    	}
    }
}
