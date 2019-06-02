using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public GameObject BridgePiece;

	public int OwnerID = 2;
	public float Points = 100f;

	public const int PlayerID = 1;

	public float[] Personality = new float[2];
	//Traits:
	//Trait 1: Agressiveness
	//Trait 2: Spendiness

	public string[] BuildStyle = new string[3];

	private const int BridgePieceCost = 1;
	private const float PointsPerRadius = 0.1f;

	// Start is called before the first frame update
	void Start()
    {
		//generate personality
		Personality[0] = Random.Range(0f, 1f);
		Personality[1] = Random.Range(0f, 1f);

		//set build style
		BuildStyle = GenerateBuildStyle(Personality[0], Personality[1]);
		//TEMP, PLEASE DELETE, YOU MORON!
		for (int i = 0; i < 3; i++)
		{
			BuildStyle[i] = "QuickAction";
		}

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
		float agPointsToSave = (GetPointsPerTime() * 100) / ag;
		float spPointsToSave = (GetPointsPerTime() * 10) / sp;

		float MinPointsSaved = ((agPointsToSave + spPointsToSave) / 2) + (GetPointsPerTime() * 100 * Random.Range(-1f, 1f));

		if (Points > MinPointsSaved)
		{

			//int Choice = Random.Range(1, 3);
			int Choice = 1;

			if (Choice == 1)
			{
				//build if we can otherwise do nothing
				if (Points - BridgePieceCost > MinPointsSaved)
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
		if (Points > BridgePieceCost)
		{
			string BuildChoice = BuildStyle[Random.Range(0, BuildStyle.Length)];

			Debug.Log(BuildChoice);

			if (BuildChoice == "CrazyAction")
			{
				CrazyAction();
			}
			else if (BuildChoice == "AggressiveAction")
			{
				AggressiveAction();
			}
			else if (BuildChoice == "PlanningAction")
			{
				PlanningAction();
			}
			else if (BuildChoice == "QuickAction")
			{
				QuickAction();
			}
			else if (BuildChoice == "GreedyAction")
			{
				GreedyAction();
			}
			else if ((BuildChoice == "GoalAction"))
			{
				GoalAction();
			}
		}
	}

	//_____________________Placement behaviors - These decide WHERE to build the new bridge unit then they call Build() with that information__________________
	public void CrazyAction() //A. ag: 1 sp: 6
	{
		//random placement
	}

	public void AggressiveAction() //B. ag: 6 sp: 4
	{
		//placement towards player's closest node (From my node with the lowest distance to any player node)

		GameObject ClosestBridgePiece = null;
		float SmallestDelta = -1f;

		//loop trough all my bridge units find out how far each is from its nearest player node and save that delta
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgePiece"))
		{
			if (obj.GetComponent<BridgeScript>().GetOwner() == OwnerID)
			{
				Vector3 ClosestPlayernodeCenterPointPosition = GetPlayerNodeClosestToPoint(obj.transform.position).transform.position;

				float CurrentUnitDelta = Vector3.Distance(ClosestPlayernodeCenterPointPosition, obj.transform.position);

				if (CurrentUnitDelta < SmallestDelta)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
				}
			}
		}


		if (ClosestBridgePiece != null)
		{
			GameObject ClosestPlayerNode = GetPlayerNodeClosestToPoint(ClosestBridgePiece.transform.position);
			GameObject snapPoint = GetSnapPointClosestToPoint(ClosestBridgePiece, ClosestPlayerNode.transform.position);
			Build(snapPoint.transform.position, snapPoint.transform.position - GetChildObjectWithTag(ClosestBridgePiece.transform, "CenterPoint").transform.position);
		}
		else
		{
			//find closest node to player and place in correct direction
			GameObject ClosestNode = null;
			float SmallestNodeDelta = -1f;

			//loop trough all my nodes find out how far each is from its nearest player node and save that delta
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
			{
				if (obj.GetComponent<NodeScript>().GetOwner() == OwnerID)
				{
					Vector3 ClosestPlayernodeCenterPointPosition = GetPlayerNodeClosestToPoint(obj.transform.position).transform.position;

					float CurrentNodeDelta = Vector3.Distance(ClosestPlayernodeCenterPointPosition, obj.transform.position);

					if (CurrentNodeDelta < SmallestNodeDelta)
					{
						SmallestNodeDelta = CurrentNodeDelta;
						ClosestNode = obj;
					}
					else if (SmallestNodeDelta == -1f)
					{
						SmallestNodeDelta = CurrentNodeDelta;
						ClosestNode = obj;
					}
				}
			}

			if (ClosestNode != null)
			{
				GameObject ClosestPlayerNode = GetPlayerNodeClosestToPoint(ClosestNode.transform.position);

				//Get positions
				Vector3 TargetPos = ClosestPlayerNode.transform.position;
				Vector3 ThisNode = ClosestNode.transform.position;

				//Find vector between positions
				Vector3 Direction = Vector3.Normalize(TargetPos - ThisNode);

				//Scale according to radius
				Vector3 NewPos = ThisNode + ClosestNode.GetComponent<NodeScript>().GetRadius() * Direction * 5;

				Vector3 rotation = TargetPos - ClosestNode.transform.position;

				Build(NewPos, rotation);
			}

		}
	}

	public void PlanningAction() //C. ag: 2 sp: 2
	{
		//Placement towards highest value AND closest empty node

	}

	public void QuickAction() //D. ag: 4 sp: 3
	{
		//Placement toward closest empty node
		GameObject ClosestBridgePiece = null;
		GameObject ClosestEmptyNode = null;
		float SmallestDelta = -1f;

		//loop trough all my bridge units find out how far each is from its nearest empty node and save that delta
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgePiece"))
		{
			if (obj.GetComponent<BridgeScript>().GetOwner() == OwnerID)
			{
				GameObject EmptyNode = GetEmptyNodeClosestToPoint(obj.transform.position);

				Vector3 EmptyNodeCenterPointPosition = EmptyNode.transform.position;

				float CurrentUnitDelta = Vector3.Distance(EmptyNodeCenterPointPosition, obj.transform.position);

				if (CurrentUnitDelta < SmallestDelta)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
					ClosestEmptyNode = EmptyNode;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
					ClosestEmptyNode = EmptyNode;
				}
			}
		}

		if (ClosestEmptyNode != null)
		{
			Debug.Log(ClosestEmptyNode.transform.position);
		}
		

		if (ClosestBridgePiece != null && ClosestEmptyNode != null)
		{
			
			GameObject snapPoint = GetSnapPointClosestToPoint(ClosestBridgePiece, ClosestEmptyNode.transform.position);
			Debug.Log("snapPoint: " + snapPoint.transform.position);

			Build(snapPoint.transform.position, snapPoint.transform.position - GetChildObjectWithTag(ClosestBridgePiece.transform, "CenterPoint").transform.position);
		}
		else
		{
			//find closest node to empty and place in correct direction
			GameObject ClosestNode = null;
			float SmallestNodeDelta = -1f;

			//loop trough all my nodes find out how far each is from its nearest empty node and save that delta
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
			{
				if (obj.GetComponent<NodeScript>().GetOwner() == OwnerID)
				{
					GameObject EmptyNode = GetEmptyNodeClosestToPoint(obj.transform.position);

					Vector3 EmptyNodeCenterPointPosition = EmptyNode.transform.position;

					float CurrentNodeDelta = Vector3.Distance(EmptyNodeCenterPointPosition, obj.transform.position);

					if (CurrentNodeDelta < SmallestNodeDelta)
					{
						SmallestNodeDelta = CurrentNodeDelta;
						ClosestNode = obj;
						ClosestEmptyNode = EmptyNode;
					}
					else if (SmallestNodeDelta == -1f)
					{
						SmallestNodeDelta = CurrentNodeDelta;
						ClosestNode = obj;
						ClosestEmptyNode = EmptyNode;
					}
				}
			}

			if (ClosestNode != null)
			{
				//Get positions
				Vector3 TargetPos = ClosestEmptyNode.transform.position;
				Vector3 ClosestNodePos = ClosestNode.transform.position;

				//Find vector between positions
				Vector3 Direction = Vector3.Normalize(TargetPos - ClosestNodePos);

				//Scale according to radius
				Vector3 NewPos = ClosestNodePos + ClosestNode.GetComponent<NodeScript>().GetRadius() * Direction * 5;
				Debug.Log("Target: " + TargetPos);

				Vector3 rotation = TargetPos - ClosestNode.transform.position;

				Build(NewPos, rotation);
			}

		}
	}

	public void GreedyAction() //E. ag: 3 sp: 1
	{
		//placement toward highest value node

		GameObject HighestValueNode = null;
		float HighestRadius = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{

			float CurrentNodeRadius = obj.GetComponent<NodeScript>().GetRadius();

			if (CurrentNodeRadius > HighestRadius)
			{
				HighestRadius = CurrentNodeRadius;
				HighestValueNode = obj;
			}
			else if (HighestRadius == -1f)
			{
				HighestRadius = CurrentNodeRadius;
				HighestValueNode = obj;
			}
		}

		Debug.Log("Greedy target: " + HighestValueNode.transform.position);

		GameObject ClosestBridgePiece = null;

		ClosestBridgePiece = GetBridgePieceClosestToPoint(HighestValueNode.transform.position);

		if (ClosestBridgePiece != null)
		{
			GameObject snapPoint = GetSnapPointClosestToPoint(ClosestBridgePiece, HighestValueNode.transform.position);
			Build(snapPoint.transform.position, snapPoint.transform.position - GetChildObjectWithTag(ClosestBridgePiece.transform, "CenterPoint").transform.position);
		}
		else
		{
			GameObject ClosestNode = null;

			ClosestNode = GetNodeIOwnClosestToPoint(HighestValueNode.transform.position);

			if (ClosestNode != null)
			{

				//Get positions
				Vector3 TargetPos = HighestValueNode.transform.position;
				Vector3 ThisNode = ClosestNode.transform.position;

				//Find vector between positions
				Vector3 Direction = Vector3.Normalize(TargetPos - ThisNode);

				//Scale according to radius
				Vector3 NewPos = ThisNode + ClosestNode.GetComponent<NodeScript>().GetRadius() * Direction * 5;

				Vector3 rotation = TargetPos - ClosestNode.transform.position;

				Build(NewPos, rotation);
			}
		}

	}

	public void GoalAction() //F. ag: 5 sp: 5
	{
		//placement towards nearest goal
	}
	//________________________________________________________________________________________________________________________________________________________________

	//takes new bridge unit's location as param
	public void Build(Vector3 pos, Vector3 rot)
	{

		Debug.Log("Rot: " + rot);

		pos.z = 0;
		rot = Vector3.Normalize(rot);

		Debug.Log("Rot Normal: " + rot);

		SpendPoints(BridgePieceCost);
		GameObject NewBridge = Instantiate(BridgePiece, pos, Quaternion.identity);
		NewBridge.transform.up = rot;

		Debug.Log("Bridge up: " + NewBridge.transform.up);

		NewBridge.GetComponent<BridgeScript>().SetOwner(OwnerID);

		FindObjectOfType<AudioManager>().Play("Place Bridge Unit");

	}

	public GameObject GetNodeIOwnClosestToPoint(Vector3 point)
	{
		GameObject ClosestNode = null;
		float SmallestDelta = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			if (obj.GetComponent<NodeScript>().GetOwner() == OwnerID)
			{
				float CurrentNodeDelta = Vector3.Distance(obj.transform.position, point);

				if (CurrentNodeDelta < SmallestDelta)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
			}
		}

		return ClosestNode;
	}

	public GameObject GetPlayerNodeClosestToPoint(Vector3 point)
	{
		GameObject ClosestNode = null;
		float SmallestDelta = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			if (obj.GetComponent<NodeScript>().GetOwner() == PlayerID)
			{
				float CurrentNodeDelta = Vector3.Distance(obj.transform.position, point);

				if (CurrentNodeDelta < SmallestDelta)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
			}
		}

		return ClosestNode;
	}

	public GameObject GetEmptyNodeClosestToPoint(Vector3 point)
	{
		GameObject ClosestNode = null;
		float SmallestDelta = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			if (obj.GetComponent<NodeScript>().GetOwner() == 0)
			{
				float CurrentNodeDelta = Vector3.Distance(obj.transform.position, point);

				if (CurrentNodeDelta < SmallestDelta)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentNodeDelta;
					ClosestNode = obj;
				}
			}
		}

		return ClosestNode;
	}

	public GameObject GetBridgePieceClosestToPoint(Vector3 point)
	{
		GameObject ClosestBridgePiece = null;
		float SmallestDelta = -1f;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BridgePiece"))
		{
			if (obj.GetComponent<BridgeScript>().GetOwner() == OwnerID)
			{
				float CurrentUnitDelta = Vector3.Distance(GetChildObjectWithTag(obj.transform, "CenterPoint").transform.position, point);

				if (CurrentUnitDelta < SmallestDelta)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
				}
				else if (SmallestDelta == -1f)
				{
					SmallestDelta = CurrentUnitDelta;
					ClosestBridgePiece = obj;
				}
			}
		}

		return ClosestBridgePiece;
	}

	public GameObject GetSnapPointClosestToPoint(GameObject BridgePiece, Vector3 point)
	{
		GameObject ClosestSnapPoint = null;
		float SmallestDelta = -1f;

		if (BridgePiece.tag == "BridgePiece")
		{
			foreach (GameObject snapPoint in GameObject.FindGameObjectsWithTag("SnapPoint"))
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

	public string[] GenerateBuildStyle(float aggressiveness, float spendiness)
	{
		var BS = new List<string>();

		int[] AGBuildStyle = new int[3];
		int[] SPBuildStyle = new int[3];

		//Get the three build functions prefered by the aggressiveness (In rank order according to aggressive score)
		int AGOffset = 0;

		if (aggressiveness < 0.25f)
		{
			AGOffset = 0;
		}
		else if (aggressiveness >= 0.25f && aggressiveness < 0.5f)
		{
			AGOffset = 1;
		}
		else if (aggressiveness >= 0.5f && aggressiveness < 0.75f)
		{
			AGOffset = 2;
		}
		else
		{
			AGOffset = 3;
		}

		AGBuildStyle[0] = 1 + AGOffset;
		AGBuildStyle[1] = 2 + AGOffset;
		AGBuildStyle[2] = 3 + AGOffset;

		//Get the three build functions prefered by the aggressiveness (In rank order according to aggressive score)
		int SPOffset = 0;

		if (spendiness < 0.25f)
		{
			SPOffset = 0;
		}
		else if (spendiness >= 0.25f && spendiness < 0.5f)
		{
			SPOffset = 1;
		}
		else if (spendiness >= 0.5f && spendiness < 0.75f)
		{
			SPOffset = 2;
		}
		else
		{
			SPOffset = 3;
		}

		SPBuildStyle[0] = 1 + SPOffset;
		SPBuildStyle[1] = 2 + SPOffset;
		SPBuildStyle[2] = 3 + SPOffset;

		//Create conversion between ag and sp ranks into names of actions
		Dictionary<int, string> AggressiveRanks = new Dictionary<int, string>()
			{
				{1, "CrazyAction"},
				{2, "PlanningAction"},
				{3, "GreedyAction"},
				{4, "QuickAction"},
				{5, "GoalAction"},
				{6, "AggressiveAction"}
			};

		Dictionary<int, string> SpendRanks = new Dictionary<int, string>()
			{
				{1, "GreedyAction"},
				{2, "PlanningAction"},
				{3, "QuickAction"},
				{4, "AggressiveAction"},
				{5, "GoalAction"},
				{6, "CrazyAction"}
			};

		//Hold the names of the three functions prefered by each trait
		string[] AGActionList = new string[3];
		string[] SPActionList = new string[3];

		//set aggressive and spendy action list options to be the Actions associated with the rank values
		for (int i = 0; i < 3; i++)
		{
			AGActionList[i] = AggressiveRanks[AGBuildStyle[i]];
			SPActionList[i] = SpendRanks[SPBuildStyle[i]];
		}

		//Compare action lists and keep any values that match and then pick randomly the remaining values from the lists
		foreach (string AGaction in AGActionList)
		{
			//If an action is in both lists add it to our final list
			if (((IList<string>)SPActionList).Contains(AGaction))
			{
				BS.Add(AGaction);
			}
		}

		//If the number of actions in the final list is less than three, add actions from both action lists until we get to 3.
		while (BS.Count < 3)
		{

			var newOptions = new List<string>();

			foreach (string action in AGActionList)
			{
				newOptions.Add(action);
			}

			foreach (string action in SPActionList)
			{
				newOptions.Add(action);
			}

			string newOption = newOptions[Random.Range(0, newOptions.Count - 1)];

			if (!BS.Contains(newOption))
			{
				BS.Add(newOption);
			}

		}

		string[] BSArray = BS.ToArray();
		return BSArray;
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
