using UnityEngine;
using System.Collections;

public class CleanerQuest : Quest {

	private Statistics stats;

	public override void OnEnable () {
		base.OnEnable();
		stats = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Statistics> ();
		UpdateMenu (stats.ShipsSunk, stats.TotalEnemyShipCount);
		AICaptain.OnEnemyDied += OnEnemyDestroyed;
	}

	public override void OnDisable () {
		base.OnDisable();
		AICaptain.OnEnemyDied -= OnEnemyDestroyed;
	}


	void OnEnemyDestroyed (GameObject enemy) {
		UpdateMenu (stats.ShipsSunk, stats.TotalEnemyShipCount);
		if (AICaptain.FleetSize > 0) return;	// Still other ships alive...
		if (Port.QueueSize > 0)		 return;	// Still ships to come...

		// ... otherwise this quest has been acomplished
		IsFulfilled = true;
		IsFinished = true;
		UpdateMenu (stats.ShipsSunk, stats.TotalEnemyShipCount);

	}
}
