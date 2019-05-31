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


    //Controller variables, only one will be used
    private NodeScript nodeScript;
    private BridgeScript bridgeScript;

    public int collisions = 0;

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

    // Update is called once per frame
    void Update()
    {
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


        	//TODO: Check if the new position would overlap with an existing piece

        	//If we've clicked
        	if(Input.GetMouseButtonDown(0))
    		{
    			//We need to make sure that the click to build isn't a click to try and select another node
		
    			//Raycast and check if our click is on another node
				Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		
				
				//Raycast and check if our click is on the GUI
				pointerEventData = new PointerEventData(eventSystem);
				pointerEventData.position = Input.mousePosition;
	
				List<RaycastResult> results = new List<RaycastResult>();
				Raycaster.Raycast(pointerEventData, results);
		
    			if(!Physics2D.Raycast(MouseRay.origin, MouseRay.direction, 100) && results.Count == 0)
    			{
    				//Replace with a real bridge piece
    				Instantiate(BridgePiece, transform.position, transform.rotation);
   					Destroy(gameObject);
    			}
    			else
    			{
    				//We either clicked the GUI or something else on our screen, so it should count as us clicking off
    				Destroy(gameObject);
    			}
    		}
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

        	transform.up = Direction;
        	transform.position = closestSnapPoint.transform.position;
    	}
    	else
    	{
    		Debug.Log("Unrecognized selection!");
    		Debug.Log("Tag:" + SelectedObject.tag);
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
