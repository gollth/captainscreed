using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class Music : MonoBehaviour {

	[SerializeField] AudioClip[] songs;

	AudioSource player;

	// Use this for initialization
	void OnEnable ()
	{
		this.player = GetComponent<AudioSource> ();
		Play();
	}

	void Update () {
		if (player.isPlaying) return;
		Play();
	}

	void Play() {
		if (songs.Length == 0) { Debug.LogWarning("[" + name + "] " + GetType().Name + " has no songs to play!"); return; }
		player.clip = songs [Random.Range (0, songs.Length)];
		player.Play();
	}
}
