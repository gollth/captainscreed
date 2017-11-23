using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

	[Range(0,1)] public float progress = 0f;

	[SerializeField] RectTransform waves;
	[SerializeField] RectTransform ship;
	[SerializeField] LocaleStringSetter text;

	Generator generator;
	Fader fader;

	void OnEnable() {
		generator = GameObject.FindGameObjectWithTag("Island").GetComponent<Generator>();
		fader = GameObject.FindGameObjectWithTag("Fader").GetComponent<Fader>();
		waves.GetComponent<Image>().enabled = true;
		ship.GetComponent<Image>().enabled = true;
		text.text.enabled = true;
		generator.OnGenerationComplete += OnComplete;
		Time.timeScale = 0f;
	}
	void OnDisable() {
		generator.OnGenerationComplete -= OnComplete;
	}

	void OnComplete() {
		this.gameObject.SetActive(false);
		fader.FadeIn();
		Time.timeScale = 1f;
	}

	void OnProgress (float progress, Filter f) {
		text.key = f.Key;
		text.SyncKeyAndText();
		var size = new Vector2(Mathf.Lerp(-Screen.width, 0, progress), 0);
		waves.sizeDelta = size;
		waves.offsetMax = size;
	}

	void Update () {

		if (!Application.isPlaying) return;	
		if (!generator.Working) return;

		OnProgress(generator.Progress, generator.CurrentFilter);
	}



}
