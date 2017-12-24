using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplainCrewMenu : MonoBehaviour {

	[SerializeField] string thrustersExplanationKey = "";
	[SerializeField] string reloadersExplanationKey = "";
	[SerializeField] string repairersExplanationKey = "";
	

	void OnEnable () {
		var explainedThrusters = false;
		var explainedReloaders = false;
		var explainedRepairers = false;

		ResourceLoader.WaitFor <Crew> (tag: "Player", callback: crew => {
			var majority = Mathf.RoundToInt (crew.amount * .6f);
			crew.OnOrder.AddListener(() => {

				// Explain what reloaders do
				if (crew.Reloaders > majority && !explainedReloaders) {
					Bubble.MessageByFirstOfficer(reloadersExplanationKey);
					explainedReloaders = true;
				}

				// Explain what repairers do
				if (crew.Repairers > majority && !explainedRepairers) {
					Bubble.MessageByFirstOfficer(repairersExplanationKey);
					explainedRepairers = true;
				}

				// Expalin what thrusters do
				if (crew.Thrusters > majority && !explainedThrusters) {
					Bubble.MessageByFirstOfficer(thrustersExplanationKey);
					explainedThrusters = true;
				}
			});
		});		
	}

}
