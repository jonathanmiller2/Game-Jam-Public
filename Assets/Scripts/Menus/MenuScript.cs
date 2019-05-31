using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuScript : MonoBehaviour
{
	public GameObject OptionsMenu;
	public GameObject PauseMenu;

    public void EscapePressed()
    {	
        Debug.Log(OptionsMenu.activeSelf);
        Debug.Log(PauseMenu.activeSelf);
    	//If neither menus are open
    	if(!OptionsMenu.activeSelf && !PauseMenu.activeSelf)
    	{
    		//Open the pause menu
    		Pause();
    		PauseMenu.SetActive(true);
    	}
    	//If the pause menu is open
    	else if(PauseMenu.activeSelf)
    	{
    		//Close the pause menu
    		Unpause();
    		PauseMenu.SetActive(false);
    	}
    	//If the options menu is open
		else if(OptionsMenu.activeSelf)
    	{
    		//Go back to the pause menu
    		OptionsMenu.SetActive(false);
    		PauseMenu.SetActive(true);
    	}	
    }

    public void OptionsButtonPressed()
    {
    	//Go to the options menu
    	OptionsMenu.SetActive(true);
    	PauseMenu.SetActive(false);
    }

    public void ExitToMenuButtonPressed()
    {
    	//Return to main menu
    	SceneManager.LoadScene(0);
    }

    public void ExitToDesktopButtonPressed()
    {
    	Application.Quit();
    }

    public void BackButtonPressed()
    {
    	PauseMenu.SetActive(false);
    	Unpause();
    }

    public void OptionsBackButtonPressed()
    {
    	//Return to pause menu
    	OptionsMenu.SetActive(false);
    	PauseMenu.SetActive(true);
    }

    private void Pause()
    {
    	Time.timeScale = 0f;
    	// Debug.Log("Paused");
    }

    private void Unpause()
    {
    	Time.timeScale = 1f;
    	// Debug.Log("Unpaused");
    }
}
