using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections;


[RequireComponent(typeof (ScreenMover))]
public class Menu : MonoBehaviour {

	#region Tweakbables
	[SerializeField, Range(0,1), Tooltip("1 means, no fade at all, 0 means fade till complete background opacity")]
	float pauseFade = 0.5f;
	[SerializeField] Button continueButton;
	[SerializeField] Button nextLevelButton;
	[SerializeField] Button restartButton;
	[SerializeField] Button whiteFlagButton;
	[SerializeField] Button homeButton;
	[SerializeField] LocaleStringSetter title;
	#endregion

	#region Properties
	public enum Visibility { Visible, Transition, Invisbile }
	public Visibility Visible { get; private set; }
	#endregion

	#region References
	Fader background;
	Level level;
	Statistics statistics;
	ScreenMover mover;
	LocaleStringSetter accuracy;
	MusicBlender music;
	#endregion

	#region Construction
	void OnEnable () {  
		this.background = GameObject.Find ("UI").GetComponentInChildren<Fader> ();
		this.mover = GetComponent<ScreenMover> ();

		var gc = GameObject.FindGameObjectWithTag ("GameController");
		if (gc == null){	Debug.LogError ("Cannot find GameController Object!"); return; }
		level = gc.GetComponent<Level> ();
		statistics = gc.GetComponent<Statistics> ();
		accuracy = transform.Find("accuracy").Find("numbers").GetComponent<LocaleStringSetter>();

		music = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicBlender>();

		continueButton.onClick.AddListener (Hide);
		nextLevelButton.onClick.AddListener (() => level.Scene.StartLevelThroughFadeOut (level.NextLevel.name));
		restartButton.onClick.AddListener (() => level.Scene.StartLevelThroughFadeOut());
		whiteFlagButton.onClick.AddListener (() => level.Scene.StartTitle());
		homeButton.onClick.AddListener (() => level.Scene.StartTitle());

		Visible = Visibility.Invisbile;
		GameObject.FindGameObjectWithTag("UI").transform.Find("pause")
			.GetComponent<Button> ().onClick.AddListener (Toggle);

		level.OnFinishedAndAllGoldCollectedOnSuccess += OnLevelFinished; 
	
	}
	void OnDisable () { level.OnFinishedAndAllGoldCollectedOnSuccess -= OnLevelFinished; }

	void OnLevelFinished(bool success) {
		continueButton.gameObject.SetActive(!success);
		nextLevelButton.gameObject.SetActive (success);
		whiteFlagButton.gameObject.SetActive (false);
		homeButton.gameObject.SetActive (true);
		Show (success ? "game.menu.won" : "game.menu.lost");
	}
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) this.Toggle();
	}
	#endregion

	#region Public Interface
	public void Show (string titleKey="game.menu.pause") {
		Visible = Visibility.Transition;

		music.Pause();

        // Update Statistics
        accuracy.SetValues(statistics.Accuracy);
        
        // Show the menu
        if (title != null) { 
        	title.key = titleKey; 
        	title.SyncKeyAndText(); 
        }
		mover.Execute (() => Visible = Visibility.Visible);
		background.Fade (pauseFade);

		// Don't keep executing game, when paused
		if (titleKey == "game.menu.pause") Time.timeScale = 0;

	}

	public void Toggle () {
		switch (Visible) {
		case Visibility.Invisbile: Show (); break;
		case Visibility.Visible:   Hide (); break;
		}
	}

	public void Hide () {
		Visible = Visibility.Transition;
		music.Play();
		Time.timeScale = 1;
		background.FadeIn ();
		mover.Reverse (() => Visible = Visibility.Invisbile);
	}
	#endregion

}
