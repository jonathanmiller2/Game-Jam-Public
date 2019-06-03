using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuScript : MonoBehaviour
{
	public GameObject OptionsMenu;
	public GameObject PauseMenu;
    public GameObject TutorialMenu;

    public void EscapePressed()
    {	
    	//If neither menus are open
    	if(!OptionsMenu.activeSelf && !PauseMenu.activeSelf)
    	{
    		//Open the pause menu
    		Pause();
    		PauseMenu.SetActive(true);
			FindObjectOfType<AudioManager>().Play("Menu Play");
		}
    	//If the pause menu is open
    	else if(PauseMenu.activeSelf)
    	{
    		//Close the pause menu
    		Unpause();
    		PauseMenu.SetActive(false);
			FindObjectOfType<AudioManager>().Play("Menu Select");
		}
    	//If the options menu is open
		else if(OptionsMenu.activeSelf)
    	{
    		//Go back to the pause menu
    		OptionsMenu.SetActive(false);
    		PauseMenu.SetActive(true);
			FindObjectOfType<AudioManager>().Play("Menu Select");
		}	
        else if (TutorialMenu.activeSelf)
        {

        }
    }

    public void OptionsButtonPressed()
    {
    	//Go to the options menu
    	OptionsMenu.SetActive(true);
    	PauseMenu.SetActive(false);
		FindObjectOfType<AudioManager>().Play("Menu Select");
	}

    public void ExitToMenuButtonPressed()
    {
    	//Return to main menu
    	SceneManager.LoadScene(0);
        // IM SORRY
        GameObject.FindGameObjectWithTag("Trans").GetComponent<Animator>().gameObject.SetActive(false);
        //But I did fix this
        Unpause();
        FindObjectOfType<AudioManager>().Play("Menu Select");
	}

    public void ExitToDesktopButtonPressed()
    {
		FindObjectOfType<AudioManager>().Play("Menu Select");
		Application.Quit();
    }

    public void BackButtonPressed()
    {
    	PauseMenu.SetActive(false);
    	Unpause();
		FindObjectOfType<AudioManager>().Play("Menu Select");
	}

    public void OptionsBackButtonPressed()
    {
    	//Return to pause menu
    	OptionsMenu.SetActive(false);
    	PauseMenu.SetActive(true);
		FindObjectOfType<AudioManager>().Play("Menu Select");
	}

    private void Pause()
    {
    	Time.timeScale = 0f;
    	// Debug.Log("Paused");
    }

    private void Unpause()
    {
    	Time.timeScale = 1f;
    	 Debug.Log("Unpaused");
    }
}