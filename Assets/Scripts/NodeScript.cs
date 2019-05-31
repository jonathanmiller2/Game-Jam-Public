using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public int Owner = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //0 is friendly, nonzero is enemy
    public void setOwner(int a)
    {
        Owner = a;
    }

    public int getOwner()
    {
        return Owner;
    }
}
