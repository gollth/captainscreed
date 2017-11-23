using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicBlender : MonoBehaviour {

	[SerializeField] AudioMixer master;
	[SerializeField] AudioMixer music;
	[SerializeField] AudioMixer sound;

	string play = "Play";
	string pause = "Pause";
	string explore = "Explore";
	string combat  = "Combat";
	string victory = "Victory";

	[SerializeField, Range(0,2)] float blendingDuration = .5f;

	PlayerFollower follower;
	AudioMixerSnapshot current;

	void OnEnable() {
		follower = Camera.main.GetComponent<PlayerFollower>();
		Quest.OnAllFinished += Win;
		follower.OnEngage.AddListener(Fight);
		follower.OnDisengage.AddListener(Explore);
		current = music.FindSnapshot(explore);
	}

	void OnDisable () {
		follower.OnEngage.RemoveListener(Fight);
		follower.OnDisengage.RemoveListener(Explore);
	}

	public void Pause() {
		TransitionSnapshots(master, master.FindSnapshot(play), master.FindSnapshot(pause), blendingDuration);
	}

	public void Play() {
		TransitionSnapshots(master, master.FindSnapshot(pause), master.FindSnapshot(play), blendingDuration);
	}

	void Fight(GameObject enemy) {
		var next = music.FindSnapshot(combat);
		TransitionSnapshots(music, current, next, blendingDuration);
		current = next;
	}

	void Explore (GameObject enemy) { 
		var next = music.FindSnapshot(explore);
		TransitionSnapshots(music, current, next, blendingDuration);
		current = next;
	}
	void Win (List<Quest> quests) {
		var next = music.FindSnapshot(victory);
		TransitionSnapshots(music, current, next, blendingDuration);
		current = next;
	}


	Coroutine transitionCoroutine;
	AudioMixerSnapshot endSnapshot;
	 
	public void TransitionSnapshots(AudioMixer mixer, AudioMixerSnapshot fromSnapshot, AudioMixerSnapshot toSnapshot, float transitionTime) {
		EndTransition();
		transitionCoroutine = StartCoroutine(TransitionSnapshotsCoroutine(mixer, fromSnapshot, toSnapshot, transitionTime));
	}
	 
	IEnumerator TransitionSnapshotsCoroutine(AudioMixer mixer, AudioMixerSnapshot fromSnapshot, AudioMixerSnapshot toSnapshot, float transitionTime) {
		// transition values
		int steps = 20;
		float timeStep = (transitionTime / (float)steps);
		float transitionPercentage = 0.0f;
		float startTime = 0f;

		// set up snapshots
		endSnapshot = toSnapshot;
		AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[] {fromSnapshot, toSnapshot};
		float[] weights = new float[2];

		// stepped-transition
		for (int i = 0; i < steps; i++)
		{
			transitionPercentage = ((float)i) / steps;
			weights[0] = 1.0f - transitionPercentage;
			weights[1] = transitionPercentage;
			mixer.TransitionToSnapshots(snapshots, weights, 0f);

			// this is required because WaitForSeconds doesn't work when Time.timescale == 0
			startTime = Time.realtimeSinceStartup;
			while(Time.realtimeSinceStartup < (startTime + timeStep)) yield return null;
		}

		// finalize
		EndTransition();
	}
 
	void EndTransition() {
		if ( (transitionCoroutine == null) || (endSnapshot == null) ) return;

		StopCoroutine(transitionCoroutine);
		endSnapshot.TransitionTo(0f);
	}
}
