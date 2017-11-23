using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

	Steerman player;
	LocaleStringSetter speed;
	Image rose;

	void OnEnable() {
		speed  = GetComponentInChildren<LocaleStringSetter>();
		rose = GetComponentInChildren<Image>();
		ResourceLoader.WaitFor("Player", player => {
			this.player = player.GetComponent<Steerman>();
		});
	}


	void Update() {
		if (player == null) return;
		rose.transform.localRotation = Quaternion.Euler(0,0, player.Heading);
		speed.SetValues(Mathf.RoundToInt(2 * player.Speed));

	}
}
