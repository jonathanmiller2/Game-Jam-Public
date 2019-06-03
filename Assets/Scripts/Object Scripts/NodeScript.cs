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

    //Colors
    public Material[] NodeMaterials;
    
    public GameObject GhostBridgePiecePrefab;
    public GameObject AttackerPrefab;

    private Toggle ToggleScriptComponent;
    private InputControllerScript inputControllerScript;

    //Conversion data
    private float CheckTimer = 0f;
    private float SecondsPerCheck = 1f;

    private float ConversionTimer = 0f;
    private float SecondsForConversion = 0f;

    private float ValidAttackerRadius = .6f;
    private List<int> AttackerCountsOnNode = new List<int>() {0,0,0,0,0,0,0,0,0,0};
    private int NextOwner;

    

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

        float Dist = 1000f;
        bool tie = true;

        //Check once a second how many attackers we have on our node
        //If it's imbalanced start a conversion timer
        if(CheckTimer > SecondsPerCheck)
        {
        	CheckTimer = 0;
        	
        	//Check how many attackers are on this node
        	foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Attacker"))
        	{
        		// Debug.Log(obj.name);

        	    Dist = Vector3.Distance(obj.transform.position, gameObject.transform.position);
	
        	    if (Dist < ValidAttackerRadius)
        	    {
        	        int OwnerOfAttacker = obj.GetComponent<AttackerScript>().GetOwner();
        	        if(AttackerCountsOnNode[OwnerOfAttacker] == null)
        	        {
        	        	AttackerCountsOnNode[OwnerOfAttacker] = 1;
        	        }
        	        else
        	        {
        	        	AttackerCountsOnNode[OwnerOfAttacker] += 1;
        	        }
        	    }
        	}

        	//Find owner with highest attacker count
        	int HighestAmountOfAttackers = -1;
        	int OwnerOfHighestAttackers = 0;
        	for(int i = 0; i < AttackerCountsOnNode.Count; i++)
        	{
        		if(AttackerCountsOnNode[i] > HighestAmountOfAttackers)
        		{
        			HighestAmountOfAttackers = AttackerCountsOnNode[i];
        			OwnerOfHighestAttackers = i;
        		}
        	}
	
	
        	//Check if there's a tie
        	for(int i = 0; i < AttackerCountsOnNode.Count; i++)
        	{
        		//Don't check a tie with ourself
        		if(i != OwnerOfHighestAttackers)
        		{
        			if(AttackerCountsOnNode[i] == HighestAmountOfAttackers)
        			{
        				tie = true;
        				break;
        			}
        		}
        	}
	
        	if(!tie)
       	 	{
        		NextOwner = OwnerOfHighestAttackers;
        	}
        }
        else
        {
        	CheckTimer += Time.deltaTime;
        }

        //Check if we have an imbalance of power
        if(tie)
        {
        	NextOwner = Owner;
        	ConversionTimer = 0;
        }
        else
        {
        	if(ConversionTimer > SecondsForConversion)
        	{
        		ConversionTimer = 0;

        		//Debug.Log("Capture!");
        		//TODO: Conversion!
        		//Animation? Set material?




        		//Child 0 is the sprite displayer object
        		transform.GetChild(0).GetComponent<Animator>().SetTrigger("Capture");
        		Owner = NextOwner;
        	}
        	else
        	{
        		ConversionTimer += Time.deltaTime;
        	}
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
        		// Debug.Log("Dut");
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

    public float GetRadius()
    {
        return Radius;
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
                renderer.material = NodeMaterials[2];
            }
            else
            {
                renderer.material = NodeMaterials[NewOwner];
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

                    foreach(ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
                    {
                        if (NewOwner >= 2)
                        {
                            particleSystem.material = NodeMaterials[2];
                        }
                        else
                        {
                            particleSystem.material = NodeMaterials[NewOwner];
                        }

                        particleSystem.material.SetColor("Color_6EC6B721", newColor);
                    }

                }
            }
        }
    }

    public int GetOwner()
    {
        return Owner;
    }
}
