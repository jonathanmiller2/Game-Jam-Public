using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
*	This checks for our auxiliary inputs (such as escape) and sends them 
*   The reason we don't check input on each script that uses them is because we only want to check a key once per tick
*   If we had two different scripts both using escape, having them both check if escape is pressed would be inefficient
*/
public class MiscInputManagerScript : MonoBehaviour
{
    //In order to only input the presses down, we have to know what the previous status was
    //We're looking for edges where it goes from status up (0) to status down (1)
    private bool PreviousEscapeStatus = false;

    private InputControllerScript inputControllerScript;

	public GameObject MenusObject;

    void Start()
    {
        GameObject InputControllerManagerObject = GameObject.Find("InputController");
        inputControllerScript = InputControllerManagerObject.GetComponent<InputControllerScript>();
    }

    void Update()
    {
        
        bool EscapePressed = Input.GetButton("Escape");
        //Check if escape is pressed down
        if(EscapePressed && !PreviousEscapeStatus)
        {
            //If we have something selected, clear selection
        	if(inputControllerScript.GetSelectedObject() != null)
            {
                //Open menu
                MenuScript menuScript = (MenuScript) MenusObject.GetComponent(typeof(MenuScript));
                menuScript.EscapePressed();
            }
            else
            {
                inputControllerScript.SetSelectedObject(null);
            }
        }
        PreviousEscapeStatus = EscapePressed;
    }
}
