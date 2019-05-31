using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSongs : MonoBehaviour
{
	public string[] playSongNames = new string[10];
	public string[] fadeOutSongNames = new string[10];
	public string[] fadeInSongNames = new string[10];

	void Start()
    {

		//fade out songs to fade
		foreach (string songName in fadeOutSongNames)
		{
			FindObjectOfType<AudioManager>().FadeOut(songName);
		}


		//play songs to play
		foreach (string songName in playSongNames)
		{
			FindObjectOfType<AudioManager>().Play(songName);
		}

		//fade in songs to fade
		foreach (string songName in fadeInSongNames)
		{
			FindObjectOfType<AudioManager>().FadeIn(songName);
		}

	}

}