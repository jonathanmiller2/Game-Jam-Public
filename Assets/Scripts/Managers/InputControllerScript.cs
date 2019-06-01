﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputControllerScript : MonoBehaviour
{
	private bool previousEscapeStatus;
	
	private GraphicRaycaster Raycaster;
	private EventSystem eventSystem;


	private GameObject ClickedGameObject;
	private GameObject SelectedObject;
	private GameObject DebugTarget;
	//private bool

	void Start()
	{
		GameObject Canvas = GameObject.Find("GUI");
		GameObject GUIEventSystemObject = GameObject.Find("EventSystem");
        Raycaster = Canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GUIEventSystemObject.GetComponent<EventSystem>();		

        DebugTarget = GameObject.Find("DebugTarget");


		Cursor.visible = true;
	}

    void Update()
    {
		if (Input.GetButtonDown("BuildMode"))
		{
			SetBuildState(true);
		}
		else if (Input.GetButtonUp("BuildMode"))
		{
			SetBuildState(false);
		}


		//Handle clicking and ALL selection logic
		if(Input.GetMouseButtonDown(0))
		{
			string WhatIsClicked = WhatDidIClickOn(Input.mousePosition);

			if(WhatIsClicked == "GUI")
			{
				//Clear selection
				SelectedObject = null;
			}
			else if(WhatIsClicked == "GameObject")
			{
				if(SelectedObject && SelectedObject.tag == "Attacker" && SelectedObject.GetComponent<AttackerScript>().GetOwner() == 1)
				{
					if(ClickedGameObject.tag == "Attacker" && ClickedGameObject.GetComponent<AttackerScript>().GetOwner() == 1)
					{
						SelectedObject = ClickedGameObject;
					}
					else
					{
						SelectedObject.GetComponent<AttackerScript>().SetTarget(Input.mousePosition);
					}
				}
				else
				{
					SelectedObject = ClickedGameObject;
				}
			}
			else if(WhatIsClicked == "Nothing")
			{

				if(GameObject.Find("PlaceModeToggle").GetComponent<Toggle>().isOn)
				{
					if(SelectedObject && SelectedObject.tag == "Node")
					{
						if(SelectedObject.GetComponent<NodeScript>().GetOwner() == 1)
						{
							//We should have a ghost, as we are in build mode and we have a node or bridge selected
							GameObject.FindWithTag("GhostBridgePiece").GetComponent<GhostBridgeScript>().Build();
						}
						else
						{
							SelectedObject = null;
						}
					}
					else if(SelectedObject && SelectedObject.tag == "BridgePiece")
					{
						if(SelectedObject.GetComponent<BridgeScript>().GetOwner() == 1)
						{
							//We should have a ghost, as we are in build mode and we have a node or bridge selected
							GameObject.FindWithTag("GhostBridgePiece").GetComponent<GhostBridgeScript>().Build();
						}
						else
						{
							SelectedObject = null;
						}
					}
					else
					{
						SelectedObject = null;
					}
				}
				else
				{
					SelectedObject = null;
				}
			}
		}


		//Debug target handler (brown square)
		if(SelectedObject)
		{
			DebugTarget.transform.position = SelectedObject.transform.position;
		}
		else
		{
			DebugTarget.transform.position = new Vector3(0f,0f,0f);
		}

	}

	//This will return a string, but will also set ClickedGameObject
	public string WhatDidIClickOn(Vector3 mousePosition)
	{
		//We need to make sure that the click to build isn't a click to try and select another node
		
    	//Raycast and check if our click is on another node
		Ray MouseRay = Camera.main.ScreenPointToRay(mousePosition);
		
		
		//Raycast and check if our click is on the GUI
		PointerEventData pointerEventData = new PointerEventData(eventSystem);
		pointerEventData.position = mousePosition;
	
		RaycastHit2D raycastHit = Physics2D.Raycast(MouseRay.origin, MouseRay.direction, 100);
		
    	if(raycastHit && raycastHit.transform && raycastHit.transform.gameObject)
    	{
    		ClickedGameObject = raycastHit.transform.gameObject;
    		return "GameObject";
    	}

    	List<RaycastResult> results = new List<RaycastResult>();
		Raycaster.Raycast(pointerEventData, results);

    	if(results.Count != 0)
    	{
    		return "GUI";
    	}

    	return "Nothing";
	}

	// Should ONLY be used from MiscInputManager for escape handling
	public void SetSelectedObject(GameObject Selection)
	{
		SelectedObject = Selection;
	}

	public GameObject GetSelectedObject()
	{
		return SelectedObject;
	}

	public void SetBuildState(bool newState)
	{
		GameObject.Find("PlaceModeToggle").GetComponent<Toggle>().isOn = newState;
	}

}


			