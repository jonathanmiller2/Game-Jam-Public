using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{

	public int Owner = 1;

	//Materials for appearance change
	public Material[] AttackerMaterials;

	//If our pathfinder can't get to the desired target, then we find whatever is closest and that's desiredtarget
	private Vector3 DesiredTarget;
	//GameObject ActualTarget;

    const float NodeConnectedRadius = 1f;
    const float BridgeConnectedRadius = 1f;

	private float MoveWait = .5f;

    private float Health = 3;

    private bool Moving = false;
	
    private float HealthTickTimer = 0;
    private float SecondsPerHealthTick = 3;
    private float HealthTickRadius = 1f;

    // Start is called before the first frame update
    void Start()
    {
		RefreshAppearance();
	}

    // Update is called once per frame
    void Update()
    {
        //TODO: Attacking and crap

        if(HealthTickTimer > SecondsPerHealthTick)
        {
            HealthTickTimer = 0;
            float dist;

            //Tick health of bridge
            foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
            {
                dist = Vector3.Distance(GetChildObjectWithTag(bridge.transform, "CenterPoint").transform.position, transform.position);
                if(dist < HealthTickRadius)
                {
                    if(bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
                    {
                        bridge.GetComponent<BridgeScript>().GiveHealth(.5f);
                    }
                    else
                    {
                        bridge.GetComponent<BridgeScript>().TakeHealth(1);
                    }
                }
            }

            foreach(GameObject attacker in GameObject.FindGameObjectsWithTag("Attacker"))
            {
                dist = Vector3.Distance(attacker.transform.position, gameObject.transform.position);
                
                if(dist < HealthTickRadius && attacker.GetComponent<AttackerScript>().GetOwner() != Owner)
                {
                    attacker.GetComponent<AttackerScript>().TakeHealth(1);
                }
            }
        }
        else
        {
            HealthTickTimer += Time.deltaTime;
        }


        if(Health <= 0)
        {
			FindObjectOfType<AudioManager>().Play("Attacker Death");
			Destroy(gameObject);
        }
    }



    public void SetTarget(Vector3 newTarget)
    {
        Moving = false;
        StopAllCoroutines();
    	DesiredTarget = newTarget;

    	//Calculate actual target (closest friendly thing)
        float SmallestDist = float.MaxValue;
        GameObject ActualTarget = null;
        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            if (bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
            {
                //Check dist
                float dist = Vector3.Distance(GetChildObjectWithTag(bridge.transform, "CenterPoint").transform.position, newTarget);
                if(dist < SmallestDist)
                {
                    ActualTarget = bridge;
                    SmallestDist = dist;
                }
            }
        }
        foreach(GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            float dist = Vector3.Distance(node.transform.position, newTarget);
            if(dist < SmallestDist)
            {
                ActualTarget = node;
                SmallestDist = dist;
            }
        }

        // Debug.Log("Moving to object:" + ActualTarget.name);

        List<GameObject> Path = Dijkstra(ActualTarget);

        //If we found a valid path
        if(Path == null)
        {
            Debug.Log("Dijkstras returned null, can't reach?");
        }
        else
        {
            IEnumerator coro = MoveToTarget(Path);
            StartCoroutine(coro);
        }
    }

    public int GetOwner()
    {
    	return Owner;
    }

    public void SetOwner(int NewOwner)
    {
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
				renderer.material = AttackerMaterials[2];
			}
			else
			{
				renderer.material = AttackerMaterials[Owner];
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

					foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
					{
						renderer.material.SetColor("Color_6EC6B721", newColor);
					}

					foreach (ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
					{
						//material 2 is enemy material
						particleSystem.material = AttackerMaterials[2];
						particleSystem.material.SetColor("Color_6EC6B721", newColor);
					}

				}
			}
		}
		else if (Owner == 1)
		{
			foreach (ParticleSystemRenderer particleSystem in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
			{
				//can be owner or nuetral material
				particleSystem.material = AttackerMaterials[Owner];
			}
		}
	}

	private List<GameObject> Dijkstra(GameObject ActualTarget)
    {
        Dictionary<GameObject, float>  UnvisitedVertices = new Dictionary<GameObject, float>();
        Dictionary<GameObject, GameObject> prev = new Dictionary<GameObject, GameObject>();
        GameObject InitialObject;

        //Construct our dictionary, also find which object in the dictionary we're currently closest to (initial node)
        float SmallestDist = float.MaxValue;
        InitialObject = null;
        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            if (bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
            {
                UnvisitedVertices[bridge] = float.MaxValue;

                //Check dist
                float dist = Vector3.Distance(GetChildObjectWithTag(bridge.transform, "CenterPoint").transform.position, transform.position);
                if(dist < SmallestDist)
                {
                    InitialObject = bridge;
                    SmallestDist = dist;
                }
            }
        }
        foreach(GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            UnvisitedVertices[node] = float.MaxValue;

            float dist = Vector3.Distance(node.transform.position, transform.position);
            if(dist < SmallestDist)
            {
                InitialObject = node;
                SmallestDist = dist;
            }         
        }
        //Vertices constructed

        //Set the tentative distance of our initial node to 0
        UnvisitedVertices[InitialObject] = 0;


        //While our unvisited vertices isn't empty
        while(UnvisitedVertices.Count != 0)
        {
            //Find vertex with lowest dist, set as current node
            float CurrentNodeTDist = float.MaxValue;
            GameObject CurrentNode = null;
            foreach(KeyValuePair<GameObject, float> entry in UnvisitedVertices)
            {
                if(entry.Value < CurrentNodeTDist)
                {
                    CurrentNodeTDist = entry.Value;
                    CurrentNode = entry.Key;
                }
            }

            if(CurrentNode == null)
            {
                Debug.Log("Couldn't find another current node");
                break;
            }

            //Remove the object we're inspecting from unvisited
            UnvisitedVertices.Remove(CurrentNode);

            //Terminate if we've found our target
            if(CurrentNode == ActualTarget)
            {
                break;
            }

            //For each neighbor of the object we're inspecting that's still unvisited VVV

            //Create a list of keys for us to iterate over
            List<GameObject> keys = new List<GameObject>(UnvisitedVertices.Keys);
            foreach(GameObject key in keys)
            {
                //If our entry is close enough to be a neighbor
                bool NodeAndConnected = key.tag == "Node" && Vector3.Distance(key.transform.position, CurrentNode.transform.position) < NodeConnectedRadius;
                bool BridgeAndConnected = key.tag == "BridgePiece" && Vector3.Distance(GetChildObjectWithTag(key.transform, "CenterPoint").transform.position, CurrentNode.transform.position) < BridgeConnectedRadius;
                if(NodeAndConnected || BridgeAndConnected)
                {
                    float AltTDist = float.MaxValue;
                    //Add the TDist of our current node and the distance 
                    if(key.tag == "BridgePiece")
                    {
                        AltTDist = CurrentNodeTDist + Vector3.Distance(GetChildObjectWithTag(key.transform, "CenterPoint").transform.position, CurrentNode.transform.position);
                    }
                    else
                    {
                        AltTDist = CurrentNodeTDist + Vector3.Distance(key.transform.position, CurrentNode.transform.position);
                    }
                    
                    //If we've found a new lower dist
                    if(AltTDist < UnvisitedVertices[key])
                    {
                        //Set the dist of our neighbor to our new dist
                        UnvisitedVertices[key] = AltTDist;
                        prev[key] = CurrentNode;
                    }
                }
            }
        }
        //First part of dijkstra's complete

        //Reading path
        List<GameObject> ShortestPath = new List<GameObject>();
        GameObject u = ActualTarget;
        if(prev.ContainsKey(u) || u == InitialObject)
        {

            while(true)
            {
                ShortestPath.Insert(0, u);
                if(prev.ContainsKey(u))
                {
                    u = prev[u];
                }
                else
                {
                    break;
                }
                
            }
            return ShortestPath;
        }
        else
        {
            return null;
        }
    }

/*

    the problem with the path list is that dijkstra's algo doesnt go in a straight line
    rather, it expands radially outward

    so any path is going to show the radial expansion, and not the actual way the attacker should move

    
    private void Dijkstra(GameObject currentNode, Dictionary<GameObject, float[]>  Vertices, GameObject DestinationObject, List<GameObject> me no worky good Path)
    {
        //Only do anything if this node hasn't already been checked

        //Mark this node as checked
        Vertices[currentNode] = new float[2]{1, Vertices[currentNode][1]};
    
        //iterate over all entries
        foreach(KeyValuePair<GameObject, float[]> entry in Vertices)
        {
            //Only look at unvisited entries
            if(entry.Value[0] == 0f)
            {
                //If our entry is close enough to be connected
                bool NodeAndConnected = entry.Key.tag == "Node" && Vector3.Distance(entry.Key.transform.position, currentNode.transform.position) < NodeConnectedRadius;
                bool BridgeAndConnected = entry.Key.tag == "BridgePiece" && Vector3.Distance(GetChildObjectWithTag(entry.Key.transform, "CenterPoint").transform.position, currentNode.transform.position) < BridgeConnectedRadius;
                
                if(NodeAndConnected || BridgeAndConnected)
                {
                    //If the new distance is lower, make the tentative distance the new distance
                    entry.Value[1] = Mathf.Min(entry.Value[1], Vector3.Distance(entry.Key.transform.position, currentNode.transform.position) + Vertices[currentNode][1]);
                }
            }
        }

        float SmallestTentativeDistance = float.MaxValue;
        GameObject ObjectWithSmallestTDistance = null;

        //iterate over all unvisited entries and find the one with the lowest tdist
        foreach(KeyValuePair<GameObject, float[]> entry in Vertices)
        {
            if(entry.Value[0] == 0f && entry.Value[1] < SmallestTentativeDistance)
            {
                SmallestTentativeDistance = entry.Value[1];
                ObjectWithSmallestTDistance = entry.Key;
            }
        }
        
    
        if(SmallestTentativeDistance == float.MaxValue || ObjectWithSmallestTDistance == null)
        {
            //If our path is dead
            return;
        }

        else if(ObjectWithSmallestTDistance == ActualTarget)
        {
            //If our path is finished
            // Path.Add(ObjectWithSmallestTDistance);
            // ValidPaths.Add(Path);
            return;
        }
        else
        {
            //If our path isn't finished
            // Path.Add(ObjectWithSmallestTDistance);
            Dijkstra(ObjectWithSmallestTDistance, Vertices, DestinationObject, Path);
        }
        
    }
*/


    IEnumerator MoveToTarget(List<GameObject> ShortestPath)
    {
        Moving = true;
        // Debug.Log(ShortestPath.Count);
        foreach(GameObject GO in ShortestPath)
        {
            yield return new WaitForSeconds(MoveWait);

            if(GO.tag == "BridgePiece")
            {
                transform.up = Vector3.Normalize(GetChildObjectWithTag(GO.transform, "CenterPoint").transform.position - transform.position);
                transform.position = GetChildObjectWithTag(GO.transform, "CenterPoint").transform.position;
            }
            else
            {
                transform.position = GO.transform.position;
            }
        }

        Moving = false;
    }

    public bool IsMoving()
    {
        return Moving;
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

    public void TakeHealth(int TakenHealth)
    {
        // Debug.Log("taken");
        Health -= TakenHealth;
    }

    public void GiveHealth(int TakenHealth)
    {
        // Debug.Log("given");
        Health += TakenHealth;
        if(Health > 2)
        {
            Health = 2;
        }
        
    }
}
