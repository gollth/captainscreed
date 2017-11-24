using UnityEngine;

using System;
using System.Linq;
using System.Collections;

public class DiscoverQuest : Quest {

	public Treasure[] checkpoints;

	int alreadyDiscovered;

	public override void OnEnable () {
		base.OnEnable();
		foreach (var checkpoint in checkpoints) 
			checkpoint.OnDiscover += OnCheckpointDiscovered;

		UpdateMenu (alreadyDiscovered, checkpoints.Length);
		
	}

	public override void OnDisable () {
		base.OnDisable();
		foreach (var checkpoint in checkpoints)
			if (checkpoint != null)
				checkpoint.OnDiscover -= OnCheckpointDiscovered;
	}



	void OnCheckpointDiscovered (int coins, Treasure.LiftType quest) {
		alreadyDiscovered += 1;
		if (alreadyDiscovered == checkpoints.Length) {
			IsFulfilled = true;
			IsFinished = true;
		}
		UpdateMenu (alreadyDiscovered, checkpoints.Length);
	}


}
