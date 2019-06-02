﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*	This script handles the options menu
*	Right now it only does the main menu, but it will most likely be used in the ingame options menu as well.
*/
public class OptionsMenuScript : MonoBehaviour
{

	public void Start()
	{
		//manualls set the vloume to the position the slider is set to by default.
		FindObjectOfType<AudioManager>().ChangeGlobalVolume(0.25f);
	}

	/**
	*	When the volume slider is moved
	*/
	public void VolumeValueChanged(float newValue)
    {
		//Change volume of all sounds
		FindObjectOfType<AudioManager>().ChangeGlobalVolume(newValue);

		//Play test sound
		FindObjectOfType<AudioManager>().Play("Menu Select");
	}

}