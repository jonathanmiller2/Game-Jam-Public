using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{

	//If our pathfinder can't get to the desired target, then we find whatever is closest and that's desiredtarget
	Vector3 DesiredTarget;
	GameObject ActualTarget;

    const float NodeConnectedRadius = 1.5f;
    const float BridgeConnectedRadius = 1.5f;

	private int Owner = 1;

    private float MoveWait = .5f;

    public Dictionary<GameObject, GameObject> ChildDict = new Dictionary<GameObject, GameObject>();
    private GameObject InitialObject;

    //A list of all of our valid paths
    private List<List<GameObject>> ValidPaths = new List<List<GameObject>>();
    private List<GameObject> ShortestPath = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Attacking and crap


    }

    public void SetTarget(Vector3 newTarget)
    {
        StopAllCoroutines();
    	DesiredTarget = newTarget;

    	//Calculate actual target (closest friendly thing)
        float SmallestDist = float.MaxValue;
        GameObject ClosestObject = null;
        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            if (bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
            {
                //Check dist
                float dist = Vector3.Distance(GetChildObjectWithTag(bridge.transform, "CenterPoint").transform.position, newTarget);
                if(dist < SmallestDist)
                {
                    ClosestObject = bridge;
                    SmallestDist = dist;
                }
            }
        }

        foreach(GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            float dist = Vector3.Distance(node.transform.position, newTarget);
            if(dist < SmallestDist)
            {
                ClosestObject = node;
                SmallestDist = dist;
            }
        }

        ActualTarget = ClosestObject;

        Debug.Log("Moving to object:" + ActualTarget.name);


        DijkstraSetup(ActualTarget);

        //If we found a valid path
        if(ValidPaths.Count != 0)
        {
            int SmallestPathLength = 1000000000;

            foreach(List<GameObject> Path in ValidPaths)
            {
                if(Path.Count < SmallestPathLength)
                {
                    SmallestPathLength = Path.Count;
                    ShortestPath = Path;
                }
            }

            StartCoroutine("MoveToTarget");
        }
        else
        {
            Debug.Log("Can't reach");
        }

        

    }

    public int GetOwner()
    {
    	return Owner;
    }

    public void SetOwner(int NewOwner)
    {
    	Owner = NewOwner;
    }

    


    private void DijkstraSetup(GameObject DestinationObject)
    {
        ShortestPath = null;
        ValidPaths = new List<List<GameObject>>();

        //Each vertex will be described with 3 values. 0: GameObject, 1:whether or not it was visited (float, unvisited:0.0, visited:1.0), 2:The Tentative distance
        Dictionary<GameObject, float[]>  Vertices = new Dictionary<GameObject, float[]>();


        //Construct our dictionary, also find which object in the dictionary we're currently closest to (initial node)
        float SmallestDist = float.MaxValue;
        InitialObject = null;
        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {
            if (bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
            {
                Vertices[bridge] = new float[]{0f, float.MaxValue};

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
            Vertices[node] = new float[]{0f, float.MaxValue};

            float dist = Vector3.Distance(node.transform.position, transform.position);
            if(dist < SmallestDist)
            {
                InitialObject = node;
                SmallestDist = dist;
            }
                  
        }

        //Set initial node as current
        GameObject Current = InitialObject;

        //Set the tentative distance of our initial node to 0
        Vertices[Current] = new float[2]{0, 0};

        Dijkstra(Current, Vertices, DestinationObject, new List<GameObject>());

    }



    
    private void Dijkstra(GameObject currentNode, Dictionary<GameObject, float[]>  Vertices, GameObject DestinationObject, List<GameObject> Path)
    {
        //Only do anything if this node hasn't already been checked

        //Mark this node as checked
        Vertices[currentNode] = new float[2]{1, Vertices[currentNode][1]};

        float SmallestTentativeDistance = float.MaxValue;
        GameObject ObjectWithSmallestTDistance = null;
    
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
    
                    if(entry.Value[1] < SmallestTentativeDistance)
                    {
                        SmallestTentativeDistance = entry.Value[1];
                        ObjectWithSmallestTDistance = entry.Key;
                    }
                }
            }
        }

        //If we've found a valid path
        if(Vertices[DestinationObject][0] == 1)
        {
            Path.Add(currentNode);
            ValidPaths.Add(Path);
            return;
        }
    
        if(SmallestTentativeDistance == float.MaxValue || ObjectWithSmallestTDistance == null)
        {
            //Dead path
            return;
        }
        else
        {
            Path.Add(currentNode);
            ChildDict[currentNode] = ObjectWithSmallestTDistance;
            Dijkstra(ObjectWithSmallestTDistance, Vertices, DestinationObject, Path);
        }

    }

    IEnumerator MoveToTarget()
    {

        Debug.Log(ShortestPath.Count);

        foreach(GameObject GO in ShortestPath)
        {
            yield return new WaitForSeconds(MoveWait);

            if(GO.tag == "BridgePiece")
            {
                transform.position = GetChildObjectWithTag(GO.transform, "CenterPoint").transform.position;
            }
            else
            {
                transform.position = GO.transform.position;
            }
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
}
