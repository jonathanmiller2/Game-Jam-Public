using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{

	private InputControllerScript inputController;

	private int lastPoints = 0;

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
				mesh.text = "" + Math.Round(inputController.GetPointsPerTime() * 100f, 3);
			}
			else if (mesh.tag == "Points")
			{
				string pointsText = Mathf.Floor(inputController.GetPoints()).ToString("f0");

				if (int.Parse(pointsText) > lastPoints)
				{
					FindObjectOfType<AudioManager>().Play("Gain Point");
					lastPoints = int.Parse(pointsText);
				}

				mesh.text = pointsText;
			}
			
		}
    }
}
