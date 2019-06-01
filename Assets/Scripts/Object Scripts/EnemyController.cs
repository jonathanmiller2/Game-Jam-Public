using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public int OwnerID;
	public float Points = 0f;

	public float[] Personality = new float[2];
	//Traits:
	//Trait 1: Agressiveness
	//Trait 2: Spendiness

	private const int BridgeUnitCost = 1;
	private const float PointsPerRadius = 0.1f;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		AquirePoints();
		DecideAction();
	}

	public void DecideAction()
	{
		//Here we decide which action type to take based on personality, PPS, and points.
		//Either place a bridge unit, spawn an attacker, target attacker

		float ag = Personality[0];
		float sp = Personality[1];

		//Number of points this personality thinks ought to be saved roughly (Different by a little every tick)
		float agPointsToSave = (GetPointsPerTime() * 1000) / ag;
		float spPointsToSave = (GetPointsPerTime() * 100) / sp;

		float MinPointsSaved = ((agPointsToSave + spPointsToSave) / 2) + (GetPointsPerTime() * 100 * Random.Range(-1f, 1f));

		if (Points > MinPointsSaved)
		{

			int Choice = Random.Range(1, 3);

			if (Choice == 1)
			{
				//build if we can otherwise do nothing
				if (Points - BridgeUnitCost > MinPointsSaved)
				{
					BuildBridgeDecide();
				}
			}
			else if (Choice == 2)
			{
				//try to attack, if I have no attackers make one if I have enough money, if I don't have a viable target then build if I can, if I can't do nothing.
			}
			else
			{
				//build attackers if I have enough money, if I don't try to attack, if I can't attack do nothing.
			}

			
		}
		

	}

	public void BuildBridgeDecide()
	{
		//decided to make a bridge
		if (Points > BridgeUnitCost)
		{

		}
	}

	//_____________________Placement behaviors - These decide WHERE to build the new bridge unit then they call Build() with that information__________________
	public void CrazyAction() //ag: 1 sp6
	{
		//random placement
	}

	public void AggressiveAction() //ag: 6 sp 4
	{
		//placement towards player's closest node
	}

	public void PlanningAction() //ag2 sp2
	{
		//Placement towards highest value AND closest empty node
	}

	public void QuickAction() //ag4 sp3
	{
		//Placement toward closest empty node
	}

	public void GreedyAction() //ag3 sp1
	{
		//placement toward highest value empty node
	}

	public void GoalAction() //ag5 sp5
	{
		//placement towards nearest goal
	}
	//________________________________________________________________________________________________________________________________________________________________

	//takes new bridge unit's location as param
	public void Build()
	{
		SpendPoints(BridgeUnitCost);
		//instantiate the new bridge piece
	}

	public GameObject GetBridgeUnitClosestToPoint(Vector3 point)
	{
		GameObject ClosestBridgeUnit = null;
		float SmallestDelta = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgeUnit"))
		{
			if (obj.GetComponent<NodeScript>().GetOwner() == OwnerID)
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

	public GameObject GetSnapPointClosestToPoint(GameObject bridgeUnit, Vector3 point)
	{
		GameObject ClosestSnapPoint = null;
		float SmallestDelta = -1f;

		if (bridgeUnit.tag == "BridgeUnit")
		{
			foreach (GameObject snapPoint in GameObject.FindGameObjectsWithTag(""))
			{

				float CurrentPointDelta = Vector3.Distance(snapPoint.transform.position, point);

				if (CurrentPointDelta < SmallestDelta)
				{
					SmallestDelta = CurrentPointDelta;
					ClosestSnapPoint = snapPoint;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentPointDelta;
					ClosestSnapPoint = snapPoint;
				}

			}
		}

		return ClosestSnapPoint;
	}

	public float GetPointsPerTime()
	{
		float PointsPerTime = 0f;
		float TotalOwnedRadius = 0f;

		//loop through all nodes to find which I own and then total up the points I should get per game tick
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node")){
			if (obj.GetComponent<NodeScript>().GetOwner() == OwnerID)
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

	//later
	public void Generate()
	{
		//set owner id
		//pick color
		//pick shape
		//pick starting node?
		//pick personality (how and when to spend points)
	}

	//---------------------------------------------------------------------------------------------------

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
