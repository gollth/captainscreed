using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigg : MonoBehaviour {

	[Range(1,5)] public int level = 1;

	int lastLevel;

	void OnEnable() {
		ActiveSails.SetActive (true);
		lastLevel = -1;
	}

	void OnDisable() {
		ActiveSails.SetActive (false);
	}

	void Update() {
		if (lastLevel == level) return;
		
		foreach (Transform child in transform) child.gameObject.SetActive (false);
		ActiveSails.SetActive (true);

		lastLevel = level;
	}

	GameObject ActiveSails { get { return transform.Find ("sail" + level).gameObject; } }

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.white;
		foreach (Transform sail in ActiveSails.transform) {
			Gizmos.DrawCube (sail.position, .25f * Vector3.one);
		}
	}

}
