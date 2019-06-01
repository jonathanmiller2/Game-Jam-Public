using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class GhostBridgeScript : MonoBehaviour
{
	public GameObject BridgePiece;

	//For Raycasting
	private PointerEventData pointerEventData;
	private GraphicRaycaster Raycaster;
	private EventSystem eventSystem;

	//Other gameobjects and scripts in scene
	private GameObject SelectedObject;
    private InputControllerScript inputControllerScript;
    private PolygonCollider2D PolygonCollider;

    //UI
    private Toggle ToggleScriptComponent;


    //Controller variables, only one will be used
    private NodeScript nodeScript;
    private BridgeScript bridgeScript;

    public int BridgePieceCollisions = 0;
	private bool CanPlace = true;
	private bool PreviousCanPlace = true;


	public const float possibleOverlap = 0.1f;

	// Start is called before the first frame update
	void Start()
    {
      	GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();

        SelectedObject = inputControllerScript.GetSelectedObject();

        GameObject Canvas = GameObject.Find("GUI");
        GameObject GUIEventSystemObject = GameObject.Find("EventSystem");
        Raycaster = Canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GUIEventSystemObject.GetComponent<EventSystem>();

        PolygonCollider = gameObject.GetComponent<PolygonCollider2D>();

        GameObject ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();

        //Find the controller for the selected object
        if(SelectedObject.tag == "Node")
    	{
    		nodeScript = SelectedObject.GetComponent<NodeScript>();
    	}
    	else if(SelectedObject.tag == "BridgePiece")
    	{
    		bridgeScript = SelectedObject.GetComponent<BridgeScript>();
    	}
    	else
    	{
    		Debug.Log("Unrecognized selection!");
    		Debug.Log("Tag:" + SelectedObject.tag);
    	}
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "BridgePiece" && other.gameObject != SelectedObject)
		{
			//if the new placement would be too far inside of another bridge count it as a collision
			if (Vector3.Distance(other.gameObject.transform.position, transform.position) < possibleOverlap)
			{
				BridgePieceCollisions++;
				if (BridgePieceCollisions > 0)
				{
					CanPlace = false;
				}
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "BridgePiece" && other.gameObject != SelectedObject)
		{
			if (Vector3.Distance(other.gameObject.transform.position, transform.position) < possibleOverlap)
			{
				BridgePieceCollisions--;
				if (BridgePieceCollisions <= 0)
				{
					CanPlace = true;
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
    	//Check if we left placing mode
    	if(!ToggleScriptComponent.isOn)
    	{
    		Destroy(gameObject);
    	}

		if (CanPlace != PreviousCanPlace)
		{
			//TODO: change skin here
			
			PreviousCanPlace = CanPlace;
		}

    	//Handle if we're building off of a node
    	if(SelectedObject.tag == "Node")
    	{

    		//Get positions
        	Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        	MousePos.z = 0;
        	Vector3 ObjectPos = SelectedObject.transform.position;
	
        	//Find vector between positions
        	Vector3 Direction = Vector3.Normalize(MousePos - ObjectPos);

        	//Scale according to radius
        	Vector3 NewPos = ObjectPos + nodeScript.GetRadius() * Direction * 5;

        	//Show the ghost at the calculated position/direction
        	transform.position = NewPos;
        	transform.up = MousePos - SelectedObject.transform.position;
        	
    	}
    	else if(SelectedObject.tag == "BridgePiece")
    	{
        	//Find closest snap point
        	List<GameObject> SnapPoints = GetChildObjectsWithTag(SelectedObject.transform, "SnapPoint");
        	float smallestDistance = 1000;
        	GameObject closestSnapPoint = null;
        	foreach(GameObject sp in SnapPoints)
        	{
        		//Get position of mouse
        		Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        		//Check if the snap point is closest to mouse
        		if(Vector3.Distance(sp.transform.position, MousePos) < smallestDistance)
        		{
        			//If so it's our new closest snap point
        			smallestDistance = Vector3.Distance(sp.transform.position, MousePos);
        			closestSnapPoint = sp;
        		}
        	}

        	GameObject CenterPoint = GetChildObjectWithTag(SelectedObject.transform, "CenterPoint");

        	Vector3 Direction = Vector3.Normalize(closestSnapPoint.transform.position - CenterPoint.transform.position);

			//If we will change position
			if (closestSnapPoint.transform.position != gameObject.transform.position)
			{
				FindObjectOfType<AudioManager>().Play("Hover Position");
			}

        	transform.up = Direction;
        	transform.position = closestSnapPoint.transform.position;

    	}
    	else
    	{
    		Debug.Log("GhostBridge is being deleted because neither a node or a bridge piece is selected");
    		Destroy(gameObject);
    		//Debug.Log("Tag:" + SelectedObject.tag);
    	}
    	
    	//If we've clicked and can place
        if(Input.GetMouseButtonDown(0) && CanPlace)
    	{
    		string WhatDidIClickOn = inputControllerScript.WhatDidIClickOn(Input.mousePosition);

    		if(WhatDidIClickOn == "Nothing")
    		{
    			//Replace with a real bridge piece
				FindObjectOfType<AudioManager>().Play("Place Bridge Unit");
				GameObject newBridgePiece = Instantiate(BridgePiece, transform.position, transform.rotation);
				inputControllerScript.SetSelectedObject(newBridgePiece);
   				Destroy(gameObject);
    		}
    		else
    		{
    			Debug.Log("We clicked on something else so we're destroying this game object");
    			Destroy(gameObject);
    		}
			
    	}

        //If the selection in input controller changes, delete this object
        if(inputControllerScript.GetSelectedObject() != SelectedObject)
        {
        	Destroy(gameObject);
        }
    }

    //Helper functions
    public List<GameObject> GetChildObjectsWithTag(Transform Parent, string Tag)
    {
    	List<GameObject> res = new List<GameObject>();
    		
        for (int i = 0; i < Parent.childCount; i++)
        {
            Transform Child = Parent.GetChild(i);
            if (Child.tag == Tag)
            {
                res.Add(Child.gameObject);
            }
            if (Child.childCount > 0)
            {
                GetChildObjectsWithTag(Child, Tag);
            }
        }

        return res;
    }

    //(single)
    public GameObject GetChildObjectWithTag(Transform Parent, string Tag)
    {
        for (int i = 0; i < Parent.childCount; i++)
        {
            Transform Child = Parent.GetChild(i);
            if (Child.tag == Tag)
            {
                return Child.gameObject;
            }
            if (Child.childCount > 0)
            {
                GetChildObjectWithTag(Child, Tag);
            }
        }

        return null;
    }

}
