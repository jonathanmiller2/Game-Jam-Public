using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerScript : MonoBehaviour
{
	bool MovingToTarget = false;
	Vector3 Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       	if(Vector3.Distance(gameObject.transform.position, Target) < .01)
       	{
       		//Path to target
       		MovingToTarget = false;
       	}
    }

    public void SetTarget(Vector3 newTarget)
    {
    	MovingToTarget = true;
    	Target = newTarget;
    }
}
