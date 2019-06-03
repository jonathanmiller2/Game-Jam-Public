using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	public GameObject OptionsMenu;
	public GameObject PlayMenu;
    public GameObject TutorialMenu;
    public GameObject Tutorial;

    public void PlayButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Menu Play");
    }

    public void ExitButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Node Loss");
		Application.Quit();
    }

    public void TutorialButtonPressed()
    {
        FindObjectOfType<AudioManager>().Play("Menu Select");


        if (Tutorial.activeSelf)
        {
            Tutorial.SetActive(false);
        }
        else
        {
            Tutorial.SetActive(true);
        }
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
