using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	public GameObject OptionsMenu;
	public GameObject PlayMenu;

    public void PlayButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Menu Play");
    }

    public void ExitButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Node Loss");
		Application.Quit();
    }

    public void OptionsButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Menu Select");

		if (OptionsMenu.activeSelf)
    	{
    		OptionsMenu.SetActive(false);
    		PlayMenu.SetActive(true);
    	}
    	else
    	{
    		OptionsMenu.SetActive(true);
    		PlayMenu.SetActive(false);
    	}
    }

    public void OptionsBackButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Menu Select");

		OptionsMenu.SetActive(false);
    	PlayMenu.SetActive(true);
    }
}
