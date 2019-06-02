using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{

	//If our pathfinder can't get to the desired target, then we find whatever is closest and that's desiredtarget
	Vector3 DesiredTarget;
	GameObject ActualTarget;

    const float NodeConnectedRadius = 1;
    const float BridgeConnectedRadius = 1;

	private int Owner = 1;

    private float MoveWait = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



    }

    public void SetTarget(Vector3 newTarget)
    {
    	DesiredTarget = newTarget;

    	//Calculate actual target (closest friendly thing)
        float SmallestDist = float.MaxValue;
        GameObject ClosestObject = null;
        foreach(GameObject bridge in GameObject.FindGameObjectsWithTag("BridgePiece"))
        {

            if (bridge.GetComponent<BridgeScript>().GetOwner() == Owner)
            {
                //Check dist
                float dist = Vector3.Distance(bridge.transform.position, newTarget);
                if(dist < SmallestDist)
                {
                    ClosestObject = bridge;
                    SmallestDist = dist;
                }
            }
        }

        foreach(GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            if (node.GetComponent<NodeScript>().GetOwner() == Owner)
            {
                float dist = Vector3.Distance(node.transform.position, newTarget);
                if(dist < SmallestDist)
                {
                    ClosestObject = node;
                    SmallestDist = dist;
                }
           }
        }

        ActualTarget = ClosestObject;

        DijkstraSetup(ActualTarget);
        StartCoroutine("MoveToTarget");

    }

    public int GetOwner()
    {
    	return Owner;
    }

    public void SetOwner(int NewOwner)
    {
    	Owner = NewOwner;
    }

    public Dictionary<GameObject, GameObject> ParentDict = new Dictionary<GameObject, GameObject>();
    private GameObject InitialObject;


    private void DijkstraSetup(GameObject DestinationObject)
    {
        ParentDict = new Dictionary<GameObject, GameObject>();

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
                float dist = Vector3.Distance(bridge.transform.position, transform.position);
                if(dist < SmallestDist)
                {
                    InitialObject = bridge;
                    SmallestDist = dist;
                }
            }
        }

        foreach(GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            if (node.GetComponent<NodeScript>().GetOwner() == Owner)
            {
                Vertices[node] = new float[]{0f, float.MaxValue};

                float dist = Vector3.Distance(node.transform.position, transform.position);
                if(dist < SmallestDist)
                {
                    InitialObject = node;
                    SmallestDist = dist;
                }
            }       
        }

        //Set initial node as current
        GameObject Current = InitialObject;

        //Set the tentative distance of our initial node to 0
        Vertices[Current] = new float[2]{0, 0};

        Dijkstra(Current, Vertices, DestinationObject);

    }

    
    private void Dijkstra(GameObject currentNode, Dictionary<GameObject, float[]>  Vertices, GameObject DestinationObject)
    {
        //Only do anything if this node hasn't already been checked
        // if(Vertices[currentNode][0] == 0)
        // {
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
                    bool BridgeAndConnected = entry.Key.tag == "BridgePiece" && Vector3.Distance(entry.Key.transform.position, currentNode.transform.position) < BridgeConnectedRadius;
                    
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
    
            //Mark this node as checked
            Vertices[currentNode] = new float[2]{1, Vertices[currentNode][1]};
    
            if(SmallestTentativeDistance == float.MaxValue || Vertices[DestinationObject][0] == 1)
            {
                return;
            }
            else
            {
                ParentDict[ObjectWithSmallestTDistance] = currentNode;
                Dijkstra(ObjectWithSmallestTDistance, Vertices, DestinationObject);
            }
        // }
    }

    IEnumerator MoveToTarget()
    {
        //Check what GameObject we're currently on

        float SmallestDist = float.MaxValue;

        GameObject CurrentlyOn = InitialObject;


        /*
        foreach(KeyValuePair<GameObject, GameObject> entry in ParentDict)
        {
            Debug.Log("Printing");
            Debug.Log(entry.Key.name);
        }
        */

        while(ParentDict[CurrentlyOn])
        {
            transform.position = ParentDict[CurrentlyOn].transform.position;
            CurrentlyOn = ParentDict[CurrentlyOn];
            yield return new WaitForSeconds(MoveWait);
        }
    }
}
