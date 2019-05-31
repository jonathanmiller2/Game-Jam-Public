using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BridgeScript : MonoBehaviour, IPointerClickHandler
{
	public GameObject GhostBridge;
	
    private InputControllerScript inputControllerScript;
    private GameObject SelectedObject;

    private Toggle ToggleScriptComponent;


    // Start is called before the first frame update
    void Start()
    {
    	//Find input controller
        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();

        GameObject ToggleButtonGameObject = GameObject.Find("PlaceModeToggle");
        ToggleScriptComponent = ToggleButtonGameObject.GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
    	
    }

    //When clicked
    public void OnPointerClick(PointerEventData InputPointerEventData)
    {
        //We need to see if we're in place mode
        if(ToggleScriptComponent.isOn)
        {
            //Tell input controller we've selected an object
            inputControllerScript.SetSelectedObject(gameObject);

            //Create a ghost bridge for the circle
            Instantiate(GhostBridge, transform.position, transform.rotation);
        }
        else
        {
            inputControllerScript.SetSelectedObject(null);
        }
    }

    public List<GameObject> GetChildObjectsWithTag(string Tag)
    {
    	List<GameObject> res = new List<GameObject>();

       	for(int i = 0; i < transform.childCount; i++)
       	{
          	Transform child = transform.GetChild(i);

           	if(child.tag == Tag)
           	{
               	res.Add(child.gameObject);
           	}
       	}
       	return res;
    }

    public GameObject GetChildObjectWithTag(string Tag)
    {
       	for(int i = 0; i < transform.childCount; i++)
       	{
          	Transform child = transform.GetChild(i);

           	if(child.tag == Tag)
           	{
               	return child.gameObject;
           	}
       	}
       	return null;
    }
}
