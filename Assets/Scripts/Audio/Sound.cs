using UnityEngine;

[System.Serializable]
public class Sound
{
	public bool loop;
	public string name;

	[Range(0f, 1f)]
	public float volume;
	[Range(0.1f, 3f)]
	public float pitch;

	public AudioClip clip;

	[HideInInspector]
	public AudioSource source;
}
