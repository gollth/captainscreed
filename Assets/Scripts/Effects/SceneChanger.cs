using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneChanger : MonoBehaviour {

	public static bool ChangingLevel { get; private set; }

	public Scene Current { get { return SceneManager.GetActiveScene (); } }	

	void OnEnable() {
		SceneManager.sceneLoaded += OnLevelLoad;
	}
	void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelLoad;
	}

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		ChangingLevel = false;
	}

	public void StartLevel (string name=null) {
		if (string.IsNullOrEmpty (name)) name = SceneManager.GetActiveScene().name;

		Fabric.Answers.Answers.LogLevelStart (name);
		Debug.Log("Starting Scene:" + name);
		
		ChangingLevel = true;
        Time.timeScale = 1;
        SceneManager.LoadScene (name);
        Port.ClearQueue();
	}
	public void StartLevelThroughFadeOut (string name = null) {
		var fader = GameObject.FindGameObjectWithTag("Fader");
		if (fader == null) { StartLevel (name); return; }
		fader.transform.SetSiblingIndex (fader.transform.parent.childCount-1);
		fader.GetComponent<Fader>().FadeOut(() => StartLevel(name));

		
	}
	public void StartTitle () { this.StartLevel("Title"); }
	public void SlowDown (float minSpeed = 0) {StartCoroutine (slowmo (minSpeed));}
	IEnumerator slowmo(float minSpeed) {
		while (!Util.Around (Time.timeScale, minSpeed, 0.05f)) {
			Time.timeScale = Mathf.Lerp (Time.timeScale, minSpeed, 2 * Util.unscaledDeltaTime);
			yield return new WaitForEndOfFrame ();
		} 
	}
}
