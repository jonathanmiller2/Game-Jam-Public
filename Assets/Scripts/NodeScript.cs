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
    public GameObject GhostBridgePiecePrefab;

    private Toggle ToggleScriptComponent;
    private InputControllerScript inputControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();

        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();
    }

    void Update()
    {
        transform.localScale = new Vector3(Radius, Radius, 1f);

        //If our node is selected
        if(inputControllerScript.GetSelectedObject() == gameObject)
        {
            //Show a highlight or something
        }
    }

    public void OnPointerClick(PointerEventData InputPointerEventData)
    {
        //We need to see if we're in place mode

        if(ToggleScriptComponent.isOn)
        {
            //Tell input controller we've selected an object
            inputControllerScript.SetSelectedObject(gameObject);

            //Create a ghost bridge for the circle
            Instantiate(GhostBridgePiecePrefab, transform.position, transform.rotation);
        }
        else
        {
            inputControllerScript.SetSelectedObject(null);
        }

    }

    public float GetRadius()
    {
        return Radius;
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
