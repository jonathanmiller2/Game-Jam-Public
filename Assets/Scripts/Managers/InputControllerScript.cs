using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputControllerScript : MonoBehaviour
{
	private bool previousEscapeStatus;

	private bool BuildState = false;
	
	private GraphicRaycaster Raycaster;
	private EventSystem eventSystem;


	private GameObject ClickedGameObject;
	private GameObject SelectedObject;
	private GameObject DebugTarget;
	//private bool

	private float Points = 0f;
	private const int BridgePieceCost = 1;
	private const float PointsPerRadius = 0.01f;

	void Start()
	{
		GameObject Canvas = GameObject.Find("GUI");
		GameObject GUIEventSystemObject = GameObject.Find("EventSystem");
        Raycaster = Canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GUIEventSystemObject.GetComponent<EventSystem>();		

        // DebugTarget = GameObject.Find("DebugTarget");

		Cursor.visible = true;

	}

    void Update()
    {
    	/*
		//Debug target handler (brown square)
		if(SelectedObject)
		{
			DebugTarget.transform.position = SelectedObject.transform.position;
		}
		else
		{
			DebugTarget.transform.position = new Vector3(0f,0f,0f);
		}
		*/


		//Get points
		AquirePoints();

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
				if(SelectedObject != null)
				{
					ClearSelectMat(SelectedObject);	
				}
				SelectedObject = null;
			}
			else if(WhatIsClicked == "GameObject")
			{
				if(SelectedObject && SelectedObject.tag == "Attacker" && SelectedObject.GetComponent<AttackerScript>().GetOwner() == 1)
				{
					if(ClickedGameObject.tag == "Attacker" && ClickedGameObject.GetComponent<AttackerScript>().GetOwner() == 1)
					{
						ClearSelectMat(SelectedObject);
						//Selection happens here
						SelectedObject = ClickedGameObject;
                        // Setting isSelected on
                    	SetSelectedObject(SelectedObject);
                    }
					else
					{
						SelectedObject.GetComponent<AttackerScript>().SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
					}
				}
				else
				{
					ClearSelectMat(SelectedObject);
					//Selection happens here
					SelectedObject = ClickedGameObject;
                    // Setting isSelected on
                    SetSelectedObject(SelectedObject);
				}
			}
			else if(WhatIsClicked == "Nothing")
			{
				if(BuildState)
				{
					if(SelectedObject && SelectedObject.tag == "Node")
					{
						if(SelectedObject.GetComponent<NodeScript>().GetOwner() == 1)
						{
							//We should have a ghost, as we are in build mode and we have a node or bridge selected
							GameObject GBP = GameObject.FindWithTag("GhostBridgePiece");
							if (GBP)
							{
								GBP.GetComponent<GhostBridgeScript>().Build();
							}
						}
						else
						{
							if(SelectedObject != null)
							{
								ClearSelectMat(SelectedObject);	
							}
							SelectedObject = null;
						}
					}
					else if(SelectedObject && SelectedObject.tag == "BridgePiece")
					{
						if(GameObject.FindWithTag("GhostBridgePiece") && SelectedObject.GetComponent<BridgeScript>().GetOwner() == 1)
						{
							//We should have a ghost, as we are in build mode and we have a node or bridge selected
							GameObject.FindWithTag("GhostBridgePiece").GetComponent<GhostBridgeScript>().Build();
						}
						else
						{
							if(SelectedObject != null)
							{
								ClearSelectMat(SelectedObject);	
							}
							SelectedObject = null;
						}
					}
					else
					{
						if(SelectedObject != null)
						{
							ClearSelectMat(SelectedObject);	
						}
						SelectedObject = null;
					}
				}
				else
				{
					if(SelectedObject != null)
					{
						ClearSelectMat(SelectedObject);	
					}
					SelectedObject = null;
				}
			}
		}


	}

	

	public float GetPointsPerTime()
	{
		float PointsPerTime = 0f;
		float TotalOwnedRadius = 0f;

		//loop through all nodes to find which I own and then total up the points I should get per game tick
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			if (obj.GetComponent<NodeScript>().GetOwner() == 1)
			{
				TotalOwnedRadius += obj.GetComponent<NodeScript>().GetRadius();
			}
		}

		PointsPerTime = TotalOwnedRadius * PointsPerRadius;
		return PointsPerTime;
	}

	public void AquirePoints()
	{
		Points += GetPointsPerTime();
	}

	public void SpendPoints(int toSpend)
	{
		Points -= toSpend;
	}

	public float GetPoints()
	{
		return Points;
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

	public void SetSelectMat(GameObject inp)
	{
		if(inp != null)
		{
			//Select new object
			foreach (SpriteRenderer renderer in inp.transform.GetComponentsInChildren<SpriteRenderer>())
        	{
        	    renderer.material.SetFloat("Vector1_63F18585", .8f);
        	}
        	foreach (ParticleSystemRenderer particleSystem in inp.GetComponentsInChildren<ParticleSystemRenderer>())
        	{
        	    particleSystem.material.SetFloat("Vector1_63F18585", .8f);
        	}
        	foreach (TrailRenderer trailrenderer in inp.GetComponentsInChildren<TrailRenderer>())
        	{
        	    trailrenderer.material.SetFloat("Vector1_63F18585", .8f);
        	}
		}	
	}

	private void ClearSelectMat(GameObject inp)
	{
		if(inp != null)
		{
			Debug.Log("Clearing");
			foreach (SpriteRenderer renderer in inp.transform.GetComponentsInChildren<SpriteRenderer>())
        	{
        	    renderer.material.SetFloat("Vector1_63F18585", 0f);
        	}
        	foreach (ParticleSystemRenderer particleSystem in inp.GetComponentsInChildren<ParticleSystemRenderer>())
        	{
        	    particleSystem.material.SetFloat("Vector1_63F18585", 0f);
        	}
        	foreach (TrailRenderer trailrenderer in inp.GetComponentsInChildren<TrailRenderer>())
        	{
        	    trailrenderer.material.SetFloat("Vector1_63F18585", 0f);
        	}
        }
	}

	// Should ONLY be used from MiscInputManager for escape handling
	public void SetSelectedObject(GameObject inp)
	{
		//Clear old selection
		ClearSelectMat(SelectedObject);

		if(inp != null)
		{
			SelectedObject = inp;

			SetSelectMat(SelectedObject);
		}
	}

	public GameObject GetSelectedObject()
	{
		return SelectedObject;
	}

	public void SetBuildState(bool newState)
	{
		BuildState = newState;

		GameObject.Find("GUI").transform.Find("HUD").transform.Find("Place Mode").gameObject.SetActive(BuildState);
		Debug.Log("Build: " + newState);
	}

	public bool GetBuildState()
	{
		return BuildState;
	}

}


			