﻿using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public string clipName;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

	public float fadeSpeed;
	[Range(0f, 1f)]
	public float targetVolume;

	public bool loop;

    [HideInInspector]
    public AudioSource source;
}
