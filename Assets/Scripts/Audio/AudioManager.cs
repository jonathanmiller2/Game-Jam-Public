using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds = new Sound[10];

	private System.Collections.Generic.List<Sound> fadingOutSounds = new System.Collections.Generic.List<Sound>();
	private System.Collections.Generic.List<Sound> fadingInSounds = new System.Collections.Generic.List<Sound>();

	public static AudioManager instance;

    private void Awake()
    {

        if(instance == null){
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
		}
    }

	public void Update()
	{
		Debug.Log("Fading length: " + fadingInSounds.Count);

		//Fade sound fading out
		for (int i = 0; i < fadingOutSounds.Count; i++)
		{

			fadingOutSounds[i].source.volume = fadingOutSounds[i].source.volume - fadingOutSounds[i].fadeSpeed;
			if (fadingOutSounds[i].source.volume == 0f)
			{
				fadingOutSounds[i].source.Stop();
				fadingOutSounds[i].volume = fadingOutSounds[i].source.volume;

				fadingOutSounds.Remove(fadingOutSounds[i]);
			}
		}

		//Fade sounds fading in
		for (int i = 0; i < fadingInSounds.Count; i++)
		{

			Debug.Log("Fading in : " + fadingInSounds[i].clipName);
			fadingInSounds[i].source.volume = fadingInSounds[i].source.volume + fadingInSounds[i].fadeSpeed;
			if (fadingInSounds[i].volume >= fadingInSounds[i].targetVolume)
			{
				fadingInSounds[i].volume = fadingInSounds[i].source.volume;

				fadingInSounds.Remove(fadingInSounds[i]);
			}
		}

	}

	public void Play(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + clipName + " not found.");
            return;
        }
        s.source.Play();
    }

	public void Stop(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		s.source.Stop();
	}

	public void FadeOut(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		fadingOutSounds.Add(s);
	}

	public void FadeIn(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}

		s.source.volume = 0f;
		s.volume = 0f;

		s.source.Play();

		fadingInSounds.Add(s);
	}

	public void Pause(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		s.source.Pause();
	}

	public void Unpause(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		s.source.UnPause();
	}

	public void StopLooping(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		s.source.loop = false;
	}

	public void StartLooping(string clipName)
	{
		Sound s = Array.Find(sounds, sound => sound.clipName == clipName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + clipName + " not found.");
			return;
		}
		s.source.loop = true;
	}

	public void ChangeGlobalVolume(float newVolume)
	{

		//Stop fading in or out anything that is
		for (int i = 0; i < fadingOutSounds.Count; i++)
		{
			fadingOutSounds.Remove(fadingOutSounds[i]);
		}

		for (int i = 0; i < fadingInSounds.Count; i++)
		{
			fadingInSounds.Remove(fadingInSounds[i]);
		}

		//Change all sound's volume
		foreach (Sound s in sounds)
		{
			s.source.volume = newVolume;
			s.targetVolume = newVolume;
		}
	}

}