using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Gate : MonoBehaviour {

	public string bubbleOnReach = "";
	TutorialQuest tutorial;

	void Start() {	// not onenable
		tutorial = GameObject.FindGameObjectWithTag("GameController").GetComponent<TutorialQuest>();
	}

	void OnTriggerExit2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		tutorial.Reach(this);
	}
}
