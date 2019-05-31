using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerScript : MonoBehaviour
{
	private bool previousEscapeStatus;

	private GameObject SelectedObject;
	//private bool

	void Start()
	{
		
		
		Cursor.visible = true;
	}

    void Update()
    {   

	}   

	public void SetSelectedObject(GameObject Selection)
	{
		SelectedObject = Selection;
	}

	public GameObject GetSelectedObject()
	{
		return SelectedObject;
	}
}
