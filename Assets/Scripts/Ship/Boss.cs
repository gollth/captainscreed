using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	public GameObject bubble;
	public string welcome;
	public string die;

	bool showed;

	// Update is called once per frame
	void Update () {
		if (showed) return;
		if (Util.IsVisibleToCamera (this.transform.position)) {
			Bubble.MessageByBoss (bubble)
				.GetComponentInChildren<LocaleStringSetter>()
				.key = welcome;
			showed = true;
		}
	}

	void OnDestroy() {
		if (Level.Shutdown || SceneChanger.ChangingLevel) return;
		
		Bubble.MessageByBoss (bubble)
			.GetComponentInChildren<LocaleStringSetter>()
			.key = die;
	}
}
