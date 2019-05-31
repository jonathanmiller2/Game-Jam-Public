using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerScript : MonoBehaviour
{
	private bool previousEscapeStatus;
	//private bool

	void Start()
	{
		Cursor.visible = true;
	}

    void Update()
    {   
    	if(true)
    	{
    		
    	}
        //Check if escape is pressed down
        //if(Input.GetButton("Escape") && previousEscapeStatus == false)
        //{
        //	//We handle what escape should actually do elsewhere
        //	MenuScript menuScript = (MenuScript) MenusObject.GetComponent(typeof(MenuScript));
        //    menuScript.EscapePressed();
        //}
        //previousEscapeStatus = Input.GetButton("Escape");
	}    
}
