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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitButtonPressed()
    {
		Application.Quit();
    }

    public void OptionsButtonPressed()
    {
    	if(OptionsMenu.activeSelf)
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
    	OptionsMenu.SetActive(false);
    	PlayMenu.SetActive(true);
    }
}
