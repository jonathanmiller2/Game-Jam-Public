﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeScript : MonoBehaviour, IPointerClickHandler
{
    [Range(0.001f, 1f)]
    public float Radius = .13f;
    public int Owner = 0;
    public float health = 100;
    
    public GameObject GhostBridgePiecePrefab;
    public GameObject AttackerPrefab;

    private Toggle ToggleScriptComponent;
    private InputControllerScript inputControllerScript;

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

        //If our node is selected
        if(inputControllerScript.GetSelectedObject() == gameObject)
        {
        		
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
    		   		//We need to see if we're in place mode
    		   		if(ToggleScriptComponent.isOn)
    		   		{
    		   		    //Tell input controller we've selected an object
    		   		    inputControllerScript.SetSelectedObject(gameObject);
		
    		   		    //Create a ghost bridge for the circle
    		   		    Instantiate(GhostBridgePiecePrefab, transform.position, transform.rotation);
    		   		}
    		   		else
    		   		{
    		   		    inputControllerScript.SetSelectedObject(null);
    		   		}
        	}
        	//If right click
        	else if(InputPointerEventData.button == PointerEventData.InputButton.Right)
        	{
        		Instantiate(AttackerPrefab, transform.position, transform.rotation);	
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