using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialQuest : Quest {

	List<Gate> checkpoints;
	Indicator indicator;
	int checkpointCount;
	GameObject backboard, starboard;
	GameObject barrel;
	PlayerFollower follower;
	string originalKey;

	public override void OnEnable () {
		base.OnEnable();
		this.IsFinished = false;
		follower = Camera.main.GetComponent<PlayerFollower>();
		checkpoints = GetComponentsInChildren<Gate>().ToList();
		checkpointCount = checkpoints.Count;
		indicator = Indicator.CreateFor(checkpoints.First().transform, friendly: true);

		ResourceLoader.WaitFor("Cockpit", ui => {
			foreach (var firer in ui.GetComponentsInChildren<ReloadIndicator>()) {
				if (backboard == null) backboard = firer.gameObject;
				else if (starboard == null) starboard = firer.gameObject;
				firer.gameObject.SetActive(false);
			}
		});

		ResourceLoader.WaitFor("Menu", menu => {
			originalKey = questInMenu.key;
			questInMenu.key += ".fail";
			questInMenu.SyncKeyAndText();
			questInMenu.transform.Find ("false").GetComponent<Image>().enabled = false;	// cannot "loose" this...
			menu.transform.Find("accuracy").gameObject.SetActive(false);
		});
	}

	public override void OnDisable () {
		base.OnDisable();
		if (indicator != null) Destroy(indicator.gameObject);
	}


	public void Reach(Gate gate) {
		if (!checkpoints.Contains(gate)) return;		// already reached this checkpoint
		if (checkpoints.IndexOf(gate) != 0) return; // not yet time to reach this checkpoint

		checkpoints.Remove(gate);
		UpdateMenu (checkpointCount-checkpoints.Count, checkpointCount);
		if (!string.IsNullOrEmpty(gate.bubbleOnReach))
			Bubble.MessageByFirstOfficer(gate.bubbleOnReach);


		
		if (checkpoints.Count > 0) {
			indicator.target = checkpoints.First().transform;
			return;
		}

		// all checkpoints reached, time to blow up the barrel
		Destroy(indicator.gameObject);
		StartCoroutine(HighlightCannons(3));

		var ship  = GetComponentInChildren<Ship>();
		barrel    = ship.gameObject;
		indicator = Indicator.CreateFor(barrel.transform, friendly: false);

		follower.OnDisengage.AddListener(e => barrel = e);
		follower.OnEngage.AddListener(e => barrel = null);
		ship.OnDestroyed   += () => {
			follower.Disengage(ship.gameObject);
			IsFulfilled = true;
			IsFinished = true;
			questInMenu.key = originalKey + ".pass";
			questInMenu.SyncKeyAndText();
		};
	}

	void Update() {
		if (barrel == null) return;
		if (!follower.InEngagementDistance(barrel.transform)) return;

		follower.Engage(barrel.gameObject);
	}

	IEnumerator HighlightCannons(int howoften=2) {

		yield return new WaitForSeconds(3f);

		backboard.SetActive(true);
		starboard.SetActive(true);

		var b = backboard.GetComponentInChildren<Button>().animator;
		var s = starboard.GetComponentInChildren<Button>().animator;


		b.SetTrigger("Show");
		s.SetTrigger("Show");
		yield return new WaitForSeconds(1f);

	}


}
