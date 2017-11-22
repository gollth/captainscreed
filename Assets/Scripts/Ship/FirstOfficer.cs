using UnityEngine;
using System.Collections;

public class FirstOfficer : MonoBehaviour {

	
	[SerializeField] string showBubbleOnStart = string.Empty;
	[SerializeField] string showBubbleOnFinish = string.Empty;


	private Level level;
	
	void OnEnable () {
		if (string.IsNullOrEmpty(showBubbleOnStart)) return;
		StartCoroutine (CreateQuestBubbleAfter (1, showBubbleOnStart));
		level = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Level> ();
		level.OnFinished += OnLevelFinished;
	}

	void OnDisable () {
		if (level != null) level.OnFinished -= OnLevelFinished;
	}

	void OnLevelFinished (bool success = true) {
		if (!success) return;
		if (string.IsNullOrEmpty (showBubbleOnFinish)) return;
		StartCoroutine (CreateQuestBubbleAfter (0, showBubbleOnFinish));
	}
	
	IEnumerator CreateQuestBubbleAfter (float duration, string bubble) {
		yield return new WaitForSeconds (duration);
		Bubble.MessageByFirstOfficer (bubble);
	}

}
