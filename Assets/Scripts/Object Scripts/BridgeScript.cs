using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BridgeScript : MonoBehaviour, IPointerClickHandler
{
	public GameObject GhostBridge;
	public int Owner = 0;

    public Material[] BridgeMaterials;


	
    private InputControllerScript inputControllerScript;
    private GameObject SelectedObject;

    private Toggle ToggleScriptComponent;


    //Support variables
    private bool Supported = true;

    private float CheckCounter = 0f;
    private const float SecondsPerCheck = 2f;

    private float UnsupportedCounter = 0f;
    private const float SecondsToAllowUnsupported = 2f;

    private const float ConnectedDistanceToNode = .5f;
    private const float ConnectedDistanceToBridge = .5f;

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

		//This should be removed eventually. Here to cure cursed geometry.
		if(transform.rotation.x != 0){
			transform.rotation = Quaternion.Euler(0, 0, 180f);
		}

        //TODO: Enemy attacker within range of our bridge, start taking damage
        //Friendly attacker within range of our bridge, heal
        //Both friendly and enemy attacker within range, do nothing (they're fighting each other)

		//Determine if our bridge is attached to something that's supported
        /*
		//Check once every couple seconds if our bridge is supported, if not, delete it
		//This check is delayed because the order that the other bridge pieces will check is unknown, so we delay to allow all of them to check
		if (CheckCounter > SecondsPerCheck)
    	{
    		Supported = CheckSupport();
    		CheckCounter = 0;
    	}
    	else
    	{
    		CheckCounter += Time.deltaTime;
    	}

        

        if(!Supported)
        {
            //If we are unsupported, we check every tick to make sure the game isn't just taking a second to chain supportedness
            //TODO: This may cause performance issues
            Supported = CheckSupport();

            if(UnsupportedCounter > SecondsToAllowUnsupported)
            {
                //TODO: Play falling/breaking animation
                Destroy(gameObject)
            }
            else
            {
                UnsupportedCounter += Time.deltaTime
            }
        }
        */
    }

    public bool CheckSupport(List<GameObject> AlreadyChecked)
    {
        
        AlreadyChecked.Add(gameObject);    
        
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            //If we own it and it's close enough
            if (node.GetComponent<NodeScript>().GetOwner() == Owner && Vector3.Distance(node.transform.position, transform.position) <= ConnectedDistanceToNode)
            {
                Supported = true;
                return Supported;
            }
        }

        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            //If we own it and it's close enough, and we haven't already checked it
            if (bridge.GetComponent<NodeScript>().GetOwner() == Owner && Vector3.Distance(bridge.transform.position, transform.position) <= ConnectedDistanceToBridge && !AlreadyChecked.Contains(bridge))
            {
                if(bridge.GetComponent<BridgeScript>().CheckSupport(AlreadyChecked))
                {
                    Supported = true;
                    return Supported;
                }
            }
        }

        //If nothing is supported
        Supported = false;
        return false;
    }

    //Added just in case (in case has happened, we need this now)
    public void SetOwner(int NewOwner)
    {
        Owner = NewOwner;

        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            //Clamp as we only have one enemy material
            if(NewOwner > 2)
            {
                renderer.material = BridgeMaterials[2];
            }
            else
            {
				renderer.material = BridgeMaterials[NewOwner];
            }
        }

        //Enemies have different colors
		if (NewOwner >= 2)
		{
			foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			{
				if (enemy.GetComponent<EnemyController>().OwnerID == NewOwner)
				{
					Color newColor = enemy.GetComponent<EnemyController>().color;

					foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
					{
						renderer.material.SetColor("Color_6EC6B721", newColor);
					}

					foreach(ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>()){
						if (NewOwner >= 2)
						{
							particleSystem.material = BridgeMaterials[2];
						}
						else
						{
							particleSystem.material = BridgeMaterials[NewOwner];
						}

						particleSystem.material.SetColor("Color_6EC6B721", newColor);
					}

				}
			}
		}
	}

    public bool GetSupportedBool()
    {
        return Supported;
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
