using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeScript : MonoBehaviour
{

    public int Owner = 0;

    private GameObject ToggleButtonGameObject;
    private Toggle ToggleScriptComponent;

    // Start is called before the first frame update
    void Start()
    {
        ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();
    }

    void OnMouseDown()
    {
        //We need to see if we're in place mode

        if(ToggleScriptComponent.isOn)
        {
            Debug.Log("A column was clicked in build mode!");
            Debug.Log(gameObject.name);
        }

    }
    //0 is friendly, nonzero is enemy
    public void SetOwner(int a)
    {
        Owner = a;
    }

    public int GetOwner()
    {
        return Owner;
    }
}
