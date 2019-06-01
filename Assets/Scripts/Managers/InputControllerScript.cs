using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputControllerScript : MonoBehaviour
{
	private bool previousEscapeStatus;
	
	private GraphicRaycaster Raycaster;
	private EventSystem eventSystem;


	private GameObject SelectedObject;
	//private bool

	void Start()
	{
		GameObject Canvas = GameObject.Find("GUI");
		GameObject GUIEventSystemObject = GameObject.Find("EventSystem");
        Raycaster = Canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GUIEventSystemObject.GetComponent<EventSystem>();		


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

	}

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
    		Debug.Log("The raycast hit a transform");
    		return raycastHit.transform.gameObject.name;
    	}

    	List<RaycastResult> results = new List<RaycastResult>();
		Raycaster.Raycast(pointerEventData, results);

    	if(results.Count != 0)
    	{
    		return "GUI";
    	}

    	return "Nothing";
	}

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


			