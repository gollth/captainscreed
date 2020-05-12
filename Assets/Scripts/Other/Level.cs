using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	#region Statics
	public const string KEY_LEVEL = "level";

	public static int Number { 
		get { return PlayerPrefs.GetInt (KEY_LEVEL, 1); }
		private set {PlayerPrefs.SetInt (KEY_LEVEL, value); PlayerPrefs.Save (); }
	}
	static void FinishLevel (Scene level) {
		int levelIndex = Util.GetLevelFromScene(level.name);
		if (Number <= levelIndex) Number = levelIndex+1;	// save the currently finished level in playerprefs
	}

	public static bool Shutdown { get; private set; }

	#endregion

	#region Tweakables
	public AudioClip victorySound;
	[Tooltip ("The time the player has left, after all quests have been achieved, e.g. to collect coins")]
	public float victoryDelay = 10;
	public float defeatDelay = 3;
	[Tooltip("The time to wait after the level was won, but the player might still die and lose nevertheless. Set this to zero, if the level is instantly won.")]
	public float uncertaintyDelay = 2;

	public SceneChanger Scene { get { return manager; } }
	#endregion

	#region Events
	/// <summary> Occurs when on this level finishes, e.g. the player has been destroyed or in
	/// the moment all quests are fullfilled (after the uncertainty delay)</summary>
	public event Action<bool> OnFinished;
	/// <summary> Occurs when this level finishes, e.g. the player has been destroyed or in
	/// the moment all quests are fullfilled, and all the gold has been collected (after the victory delay) </summary>
	public event Action<bool> OnFinishedAndAllGoldCollectedOnSuccess;
	#endregion

	#region References
	private Ship ship;
	private SceneChanger manager;

	private	Statistics statistics;
	private Goldpot goldpot;

	private Attractor attractor;
	private GameObject particles;
	#endregion

	public Scene NextLevel { get { return SceneManager.GetSceneByBuildIndex (SceneManager.GetActiveScene ().buildIndex + 1); }}

	void OnEnable () {  
		ResourceLoader.WaitFor (tag: "Player", callback: player => {
			ship = 		player.GetComponent<Ship> ();
			attractor = player.GetComponent<Attractor> ();

			var game = GameObject.FindGameObjectWithTag ("GameController");
			manager = game.GetComponent<SceneChanger>();
			statistics = game.GetComponent<Statistics> ();

			particles = GameObject.FindGameObjectWithTag ("Particles");
			goldpot = GameObject.FindGameObjectWithTag ("Goldpot").GetComponent<Goldpot> ();

			// Add the matching quest for this level
			gameObject.AddComponent(Quest.Levels[manager.Current.name]);
			

			ship.OnDestroyed += OnPlayerDestroyed;  
			Quest.OnAllFinished += OnAllQuestFinished;

		});
	}

	void OnDisable () { 
		if (ship != null) ship.OnDestroyed -= OnPlayerDestroyed;  
		Quest.OnAllFinished -= OnAllQuestFinished;
	}

	void OnAllQuestFinished (List<Quest> quests) {
		StartCoroutine (WaitForEndOfGame (
			victory: 
				quests.All (item => item.IsFulfilled), 
			callback:() => {
				FinishLevel (manager.Current);	// cache in player prefs
				manager.SlowDown (.2f);
				// Fabric.Answers.Answers.LogLevelEnd (manager.Current.name, statistics.Accuracy, true);
		}));
	}
	void OnPlayerDestroyed () { 
		StartCoroutine (WaitForEndOfGame(victory:false, callback:() => {
			if (OnFinished != null)	OnFinished(false);
			// Fabric.Answers.Answers.LogLevelEnd (manager.Current.name, statistics.Accuracy, false);
		}));
	}

	float findFarthestCoinDistance () {
		float distance = -1;
		foreach (var coin in particles.GetComponentsInChildren<Coin>()) {
			var d = (coin.transform.position - ship.transform.position).magnitude;
			if (d > distance) distance = d;
		}
		return distance;
	}

	IEnumerator WaitForEndOfGame (bool victory, System.Action callback) {
		if (victory) {
			// Wait if player might still die
			yield return new WaitForSeconds (uncertaintyDelay);
			if (ship == null) 	yield break;	// Player died yet, let OnPlayerDestroyed() call this coroutine again

			if (OnFinished != null) OnFinished (true);

			yield return new WaitForSeconds(0.5f);	// Wait until the coins have exploded
			attractor.quadratricFalloff = false;
			attractor.range = findFarthestCoinDistance() * 1.05f;
			yield return new WaitForSeconds (.25f * victoryDelay);
			if (victorySound != null) AudioSource.PlayClipAtPoint (victorySound, ship.transform.position);
			yield return new WaitForSeconds (.75f * victoryDelay);
			goldpot.StoreHoldInStowage ();
		} else {
			if (OnFinished != null)	OnFinished (false);
			yield return new WaitForSeconds (defeatDelay);
			goldpot.LooseWholeHold ();
		}

		yield return new WaitUntil (() => goldpot.TransferFinished);
		if (OnFinishedAndAllGoldCollectedOnSuccess != null)	OnFinishedAndAllGoldCollectedOnSuccess (victory);
		callback();
	}

	void OnApplicationQuit() {
		Shutdown = true;
	}

}
