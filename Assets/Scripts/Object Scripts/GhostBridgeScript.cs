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


	public const float MinimumBridgeDistance = 0.23f;
	public const float MinimumNodeDistance = 0.5f;

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
    		// Debug.Log("Unrecognized selection!");
    		Destroy(gameObject);
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
		

    	//Handle if we're building off of a node (show the ghost in a radius around the node)
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
    	//Handle if we're building off a bridge (snap to snap points)
    	else if(SelectedObject.tag == "BridgePiece")
    	{
        	//Find closest empty snap point

        	//Get snap points
        	List<GameObject> SnapPoints = GetChildObjectsWithTag(SelectedObject.transform, "SnapPoint");
        	
        	//Iterate over the snap points
        	float smallestDistance = 1000;
        	GameObject closestSnapPoint = null;
        	foreach(GameObject sp in SnapPoints)
        	{
        		//Get the script on the snap point to check if it's holding another object
        		SnapPointScript spscript = sp.GetComponent<SnapPointScript>();
        		
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

			//If we change position
			if (closestSnapPoint.transform.position != gameObject.transform.position)
			{

				//TODO: Prevent spamming of this when our ghost glitches out
				FindObjectOfType<AudioManager>().Play("Hover Position");
			}

        	transform.up = Direction;
        	transform.position = closestSnapPoint.transform.position;
    	}


        //If the selection in input controller changes due to another script, delete this object
        if(inputControllerScript.GetSelectedObject() != SelectedObject)
        {
        	Destroy(gameObject);
        }

    }

    public GameObject Build()
    {
    	//Check what the closest 
        float SmallestBridgeDist = 10000f;
        float SmallestNodeDist = 10000f;
        float Dist;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            Dist = Vector3.Distance(GetChildObjectWithTag(obj.transform, "CenterPoint").transform.position, GetChildObjectWithTag(gameObject.transform, "CenterPoint").transform.position);

            if (Dist < SmallestBridgeDist)
            {
                SmallestBridgeDist = Dist;
            }
            else if (SmallestBridgeDist == 10000f)
            {
                SmallestBridgeDist = Dist;
            }
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
        {
            Dist = Vector3.Distance(obj.transform.position, GetChildObjectWithTag(gameObject.transform, "CenterPoint").transform.position);

            if (Dist < SmallestNodeDist)
            {
                SmallestNodeDist = Dist;
            }
            else if (SmallestNodeDist == 10000f)
            {
                SmallestNodeDist = Dist;
            }
        }

        //Debug.Log(SmallestDelta);

    	if(SmallestBridgeDist > MinimumBridgeDistance && SmallestNodeDist > MinimumNodeDistance)
    	{
    		// Debug.Log("Placing with:" + BridgePieceCollisions + " BridgePieceCollisions");
    		//Replace with a real bridge piece
			FindObjectOfType<AudioManager>().Play("Place Bridge Unit");
			GameObject newBridgePiece = Instantiate(BridgePiece, transform.position, transform.rotation);
			SelectedObject = newBridgePiece;
			inputControllerScript.SetSelectedObject(newBridgePiece);
	
			return newBridgePiece;
		}
		else
		{
			/*
			if(SmallestBridgeDist <= MinimumBridgeDistance)
			{
				Debug.Log("Too Close to Bridge!");
			}
			else if(SmallestNodeDist <= MinimumNodeDistance)
			{
				Debug.Log("Too Close to Node!");
			}
			*/

			inputControllerScript.SetSelectedObject(null);
			Destroy(gameObject);
			return null;
		}
    }

/*
    public GameObject GetBridgeUnitClosestToPoint(Vector3 point)
    {
        GameObject ClosestBridgeUnit = null;
        float SmallestDelta = -1f;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            if (obj.GetComponent<BridgeScript>().GetOwner() == OwnerID)
            {
                float CurrentUnitDelta = Vector3.Distance(GetChildObjectWithTag(obj.transform, "CenterPoint").transform.position, point);

                if (CurrentUnitDelta < SmallestDelta)
                {
                    SmallestDelta = CurrentUnitDelta;
                    ClosestBridgeUnit = obj;
                }
                else if (SmallestDelta == -1f)
                {
                    SmallestDelta = CurrentUnitDelta;
                    ClosestBridgeUnit = obj;
                }
            }
        }

        return ClosestBridgeUnit;
    }
*/

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
