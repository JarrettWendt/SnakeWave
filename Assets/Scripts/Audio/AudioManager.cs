#pragma warning disable CS0649
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	// Singleton.
	public static AudioManager Instance { get; private set; }

	// A list of sounds to be set in the inspector.
	[SerializeField]
	private List<Sound> sounds;
	// A set of sounds to be used actually.
	private readonly Dictionary<string, Sound> set = new Dictionary<string, Sound>();

	#region MonoBehaviour
	void Awake()
	{
		// Singleton.
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		foreach (Sound sound in sounds)
		{
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
			set.Add(sound.name, sound);
		}
	}
	#endregion

	public void Play(string name, float delay = 0f)
	{
		if (!set.TryGetValue(name, out Sound sound))
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		if (delay == 0f)
		{
			sound.source.Play();
		}
		else
		{
			sound.source.PlayDelayed(delay);
		}
	}

	public void Stop(string name)
	{
		if (!set.TryGetValue(name, out Sound sound))
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		sound.source.Stop();
	}
}
#pragma warning restore CS0649
