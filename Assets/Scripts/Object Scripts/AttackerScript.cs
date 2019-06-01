using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{
	bool MovingToTarget = false;

	//If our pathfinder can't get to the desired target, then we find whatever is closest and that's desiredtarget
	Vector3 DesiredTarget;
	Vector3 ActualTarget;

	private int Owner = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       	if(Vector3.Distance(gameObject.transform.position, ActualTarget) < .01)
       	{
       		//Path to target
       		MovingToTarget = false;
       	}
       	else
       	{
       		MovingToTarget = true;
       	}


    }

    public void SetTarget(Vector3 newTarget)
    {
    	MovingToTarget = true;
    	DesiredTarget = newTarget;
    	

    	//Calculate actual target
    }

    public int GetOwner()
    {
    	return Owner;
    }

    public void SetOwner(int NewOwner)
    {
    	Owner = NewOwner;
    }
}
