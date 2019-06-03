using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{

	private InputControllerScript inputController;

    void Start()
    {
		inputController = FindObjectOfType<InputControllerScript>();

	}

    // Update is called once per frame
    void Update()
    {
		foreach (TextMeshProUGUI mesh in transform.GetComponentsInChildren<TextMeshProUGUI>())
		{
			if (mesh.tag == "PPS")
			{
				mesh.text = "" + inputController.GetPointsPerTime() * 10f;
			}
			else if (mesh.tag == "Points")
			{
				mesh.text = inputController.Points.ToString("f0");
			}
			
		}
    }
}
