using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BridgeScript : MonoBehaviour, IPointerClickHandler
{
	public GameObject GhostBridge;
	public int Owner = 1;

	private bool Supported = true;
	
    private InputControllerScript inputControllerScript;
    private GameObject SelectedObject;

    private Toggle ToggleScriptComponent;

    private float TimeCounter = 0f;
    private const float SecondsPerCheck = 2f;


    // Start is called before the first frame update
    void Start()
    {
    	//Find input controller
        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();

        GameObject ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
		//Determine if our bridge is attached to something that's supported

		//Check to see if we're attached to a node


		//Check once every couple seconds if our bridge is supported, if not, delete it
		//This check is delayed because the order that the other bridge pieces will check is unknown, so we delay to allow all of them to check
		if (TimeCounter > SecondsPerCheck)
    	{
    		if(!Supported)
    		{
    			//TODO: Play breaking animation
    			Destroy(gameObject);
    		}
    		TimeCounter = 0;
    	}
    	else
    	{
    		TimeCounter += Time.deltaTime;
    	}
    }

    //Added just in case (in case has happened, we need this now)
    public void SetOwner(int NewOwner)
    {
        Owner = NewOwner;

		if (Owner != 0 && Owner != 1)
		{
			foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			{
				if (enemy.GetComponent<EnemyController>().OwnerID == Owner)
				{
					Color newColor = enemy.GetComponent<EnemyController>().color;
					Material newMat = null;

					foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
					{
						if (mat.name == "Enemy")
						{
							newMat = mat;
						}
					}

					foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
					{
						Debug.Log(renderer);
						renderer.material = newMat;
						renderer.material.SetColor("Color_6EC6B721", newColor);
					}
				}
			}
		}
		else if (Owner == 1)
		{
			Material newMat = null;

			foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
			{
				if (mat.name == "Player")
				{
					newMat = mat;
				}
			}

			if (newMat)
			{
				foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
				{
					renderer.material = newMat;
					//renderer.material.color = newColor;
				}
			}
		}
		else
		{
			//SetMaterial(NutralMaterial);
		}

	}

    public int GetOwner()
    {
    	return Owner;
    }

    //When clicked
    public void OnPointerClick(PointerEventData InputPointerEventData)
    {
    	//We can only select pieces we own
        if(Owner == 1)
        {
            //If left click
            if (InputPointerEventData.button == PointerEventData.InputButton.Left)
            {
                //If we're trying to move an attacker to this bridge piece
                if(inputControllerScript.GetSelectedObject() && inputControllerScript.GetSelectedObject().tag == "Attacker")
                {
                    inputControllerScript.GetSelectedObject().GetComponent<AttackerScript>().SetTarget(transform.position);
                }
                //If we're trying to select the bridge
                else
                {
                    //We need to see if we're in place mode and we don't already have a ghost
                    if(ToggleScriptComponent.isOn && !GameObject.FindWithTag("GhostBridgePiece"))
                    {
                        //Create a ghost bridge for the circle
                        Instantiate(GhostBridge, transform.position, transform.rotation);
                    }
                }
            }
        }
        else
        {
        	//We've selected an enemy bridge!
        	//TODO: Whatever happens when you select an enemy bridge
        }
    }

	public List<GameObject> GetChildObjectsWithTag(string Tag)
    {
    	List<GameObject> res = new List<GameObject>();

       	for(int i = 0; i < transform.childCount; i++)
       	{
          	Transform child = transform.GetChild(i);

           	if(child.tag == Tag)
           	{
               	res.Add(child.gameObject);
           	}
       	}
       	return res;
    }

    public GameObject GetChildObjectWithTag(string Tag)
    {
       	for(int i = 0; i < transform.childCount; i++)
       	{
          	Transform child = transform.GetChild(i);

           	if(child.tag == Tag)
           	{
               	return child.gameObject;
           	}
       	}
       	return null;
    }
}
