using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour {

	public AudioClip[] sounds;
	[Range(0,5)] public float delay = 0;

	AudioSource source;

	void OnEnable () {
		source = GetComponent<AudioSource>();
		source.clip = sounds[Random.Range(0,sounds.Length-1)];
		source.PlayDelayed(delay);
	}
	
}
