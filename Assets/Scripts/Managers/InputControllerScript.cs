using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
		if (Input.GetButtonDown("BuildMode"))
		{
			SetBuildState(true);
		}
		else if (Input.GetButtonUp("BuildMode"))
		{
			SetBuildState(false);
		}
	}   

	public void SetSelectedObject(GameObject Selection)
	{
		SelectedObject = Selection;
	}

	public GameObject GetSelectedObject()
	{
		return SelectedObject;
	}

	public void SetBuildState(bool newState)
	{
		GameObject.Find("PlaceModeToggle").GetComponent<Toggle>().isOn = newState;
	}

}
