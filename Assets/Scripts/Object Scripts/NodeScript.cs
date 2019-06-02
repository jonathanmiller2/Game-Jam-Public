using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeScript : MonoBehaviour, IPointerClickHandler
{
    [Range(0.001f, 1f)]
    public float Radius = .13f;
    public int Owner = 0;
    
    public GameObject GhostBridgePiecePrefab;
    public GameObject AttackerPrefab;

    private Toggle ToggleScriptComponent;
    private InputControllerScript inputControllerScript;

    //Conversion data
    private float CheckTimer = 0f;
    private float SecondsPerCheck = 1f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();

        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();
    }

    void Update()
    {
        transform.localScale = new Vector3(Radius, Radius, 1f);

        //TODO: 

        //Check once a second how many attackers we have on our node
        //If it's imbalanced start a conversion timer

        if(CheckTimer > SecondsPerCheck)
        {
        	CheckTimer = 0;
        	
        	//Check how many attackers are on this node


        }
        else
        {
        	CheckTimer += Time.deltaTime;
        }        
    }

    public void OnPointerClick(PointerEventData InputPointerEventData)
    {
    	//Do we own the node?
    	if(Owner == 1)
    	{
    		//If left click
    		if (InputPointerEventData.button == PointerEventData.InputButton.Left)
    		{
    		   	//We need to see if we're in place mode and we don't have a ghost bridge piece
    		   	if(ToggleScriptComponent.isOn && !GameObject.FindWithTag("GhostBridgePiece"))
    		   	{
    		   	    //Create a ghost bridge for the circle
    		   	    Instantiate(GhostBridgePiecePrefab, transform.position, transform.rotation);
    		   	}
        	}
        	//If right click
        	else if(InputPointerEventData.button == PointerEventData.InputButton.Right)
        	{
        		Debug.Log("Dut");
        		GameObject NewAttacker = Instantiate(AttackerPrefab, transform.position, transform.rotation);	
        		inputControllerScript.SetSelectedObject(NewAttacker);
        	}
        }
        else
        {
        	//We clicked on this node and we don't own it
        	//TODO: attack
        }
    }

    public float GetRadius()
    {
        return Radius;
    }

    //0 is friendly, nonzero is enemy
    public void SetOwner(int a)
    {
        Owner = a;
    }

    public int GetOwner()
    {
        return Owner;
    }
}
