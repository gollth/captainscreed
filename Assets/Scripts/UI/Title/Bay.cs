using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Bay : MonoBehaviour {

	[SerializeField] string keyPrefix = "title.campaign.details";
	[SerializeField] LocaleStringSetter title;
	[SerializeField] LocaleStringSetter details;
	[SerializeField] LocaleStringSetter quest;
	[SerializeField] Button fight; 

	string letter = null;
	SceneChanger changer;

	void OnEnable() {
		changer = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneChanger>();
	}

	public void ShowDetails(string letter) {
		title.key   = keyPrefix + ".bay." + letter;
		details.key = keyPrefix + ".bay." + letter + ".description";
		quest.key   = keyPrefix + ".quest." + Quest.Levels[letter].Name.ToLower().Replace("quest", "");
		title.SyncKeyAndText();
		quest.SyncKeyAndText();
		details.SyncKeyAndText();

		var check = quest.transform.Find ("true");
		if (check != null) check.gameObject.SetActive(Level.Number >= Util.GetLevelFromScene(letter));

		this.letter = letter;
	}

	public void Fight() {
		changer.StartLevelThroughFadeOut(letter);
	}

}
