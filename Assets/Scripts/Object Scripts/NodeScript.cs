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

    private InputControllerScript inputControllerScript;

    //Conversion data
    private float CheckTimer = 0f;
    private float SecondsPerCheck = 1f;

    private float ConversionTimer = 0f;
    private float SecondsForConversion = 2f;

    private float ValidAttackerRadius = .6f;
    private List<int> AttackerCountsOnNode = new List<int>() {0,0,0,0,0,0,0,0,0,0};
    private int NextOwner = -1;

    private bool tie = true;

    private int AttackerPointCost = 5;

    // Start is called before the first frame update
    void Start()
    {

        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();

		RefreshAppearance();
	}

    void Update()
    {
        transform.localScale = new Vector3(Radius, Radius, 1f);

        float Dist = 1000f;


        //Check once a second how many attackers we have on our node
        //If it's imbalanced start a conversion timer
        if(CheckTimer > SecondsPerCheck)
        {
            AttackerCountsOnNode = new List<int>() {0,0,0,0,0,0,0,0,0,0};
            NextOwner = -1;
            tie = false;

        	CheckTimer = 0;
        	
        	//Check how many attackers are on this node
        	foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Attacker"))
        	{
        		// Debug.Log(obj.name);

        	    Dist = Vector3.Distance(obj.transform.position, gameObject.transform.position);
	
        	    if (Dist < ValidAttackerRadius)
        	    {
        	        int OwnerOfAttacker = obj.GetComponent<AttackerScript>().GetOwner();
        	        AttackerCountsOnNode[OwnerOfAttacker] += 1;

        	    }
        	}

        	//Find owner with highest attacker count
        	int HighestAmountOfAttackers = -1;
        	int OwnerOfHighestAttackers = -1;
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

        //Check if we have an balance of power
        if(tie)
        {
            //If we have a tie, clear our next owner and reset conversion timer
        	NextOwner = Owner;
        	ConversionTimer = 0;
        }
        //Check if we have an imbalance of power
        else
        {
            //Check if the imbalance is caused by someone other than ourselves
            if(NextOwner != Owner)
            {
                if(ConversionTimer > SecondsForConversion)
                {
                    ConversionTimer = 0;
    
                    //Child 0 is the sprite displayer object
                    transform.GetChild(0).GetComponent<Animator>().SetTrigger("Capture");
                    SetOwner(NextOwner);
                    Owner = NextOwner;
                }
                else
                {
                    ConversionTimer += Time.deltaTime;
                }
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
    		   	if(inputControllerScript.GetBuildState() && !GameObject.FindWithTag("GhostBridgePiece"))
    		   	{
    		   	    //Create a ghost bridge for the circle
    		   	    Instantiate(GhostBridgePiecePrefab, transform.position, transform.rotation);
    		   	}
        	}
        	//If right click
        	else if(InputPointerEventData.button == PointerEventData.InputButton.Right)
        	{
                if(inputControllerScript.GetPoints() > AttackerPointCost)
                {
                    inputControllerScript.SpendPoints(AttackerPointCost);
                    GameObject NewAttacker = Instantiate(AttackerPrefab, transform.position, transform.rotation);
                    NewAttacker.GetComponent<AttackerScript>().SetOwner(1);
                    inputControllerScript.SetSelectedObject(NewAttacker);
                    FindObjectOfType<AudioManager>().Play("Player Attacker Spawn");
                }
        		
			}
        }
        else
        {
            //TODO: CANT AFFORD SOUND
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
		//Play sounds before changing owner so we know old and new.

		if (Owner != 1 && NewOwner == 1)
		{
			FindObjectOfType<AudioManager>().Play("Node Gain");
		}
		else if (Owner == 1 && NewOwner != 1)
		{
			FindObjectOfType<AudioManager>().Play("Node Loss");
		}
		else
		{
			FindObjectOfType<AudioManager>().Play("Node Change");
		}
		

		Owner = NewOwner;

		RefreshAppearance();
	}

	public void RefreshAppearance()
	{
		foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
		{
			//Clamp as we only have one enemy material
			if (Owner > 2)
			{
				renderer.material = NodeMaterials[2];
			}
			else
			{
				renderer.material = NodeMaterials[Owner];
			}
		}

		//Enemies have different colors
		if (Owner > 1)
		{
			foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			{
				if (enemy.GetComponent<EnemyController>().OwnerID == Owner)
				{
					Color newColor = enemy.GetComponent<EnemyController>().color;

					Debug.Log("Enemy: " + Owner + " Color: " + enemy.GetComponent<EnemyController>().color + " PPS: " + enemy.GetComponent<EnemyController>().GetPointsPerTime());

					foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
					{
						renderer.material.SetColor("Color_6EC6B721", newColor);
					}

					foreach (ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
					{
						//material 2 is enemy material
						particleSystem.material = NodeMaterials[2];
						particleSystem.material.SetColor("Color_6EC6B721", newColor);
					}

				}
			}
		}
		else if(Owner == 1)
		{
			foreach (ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
			{
				//can be owner or nuetral material
				particleSystem.material = NodeMaterials[Owner];
			}
		}
	}

	public int GetOwner()
    {
        return Owner;
    }
}
