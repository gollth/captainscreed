using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Linq;
using System.Collections.Generic;

public class Crew : MonoBehaviour {
	#region Tweakables
	public int amount = 5;
	[Range(0, .5f)] public float thrustPerSailor = .05f;
	[Range(0, 2f)]  public float repairsPerSailor = 1f;
	[Range(0, 2f)]  public float reloadsPerSailor = .5f;
	public bool spawnCrewMenuOnStart = true;
	#endregion

	#region Properties
	public int Thrusters { get; private set; }
	public int Reloaders { get; private set; }
	public int Repairers { get; private set; }
	#endregion

	#region Events
	public UnityEvent OnOrder = new UnityEvent();
	#endregion

	#region References
	Cannons backboard, steuerboard;
	GameObject crew;
	Ship ship;
	Steerman steerman;
	#endregion

	#region Methods
	// Use this for initialization
	void OnEnable () {
		this.backboard = Cannons.OnBackboardFrom (this.gameObject);
		this.steuerboard = Cannons.OnSteuerboardFrom (this.gameObject);
		this.ship = GetComponent<Ship> ();
		this.steerman = GetComponent<Steerman> ();

		// Add the crew menu
		if (!spawnCrewMenuOnStart) return;
		crew = Instantiate (Resources.Load ("UI/crew")) as GameObject;
		crew.transform.SetParent(this.transform, false);
	
	}

	void OnDisable() {
		Destroy (crew);
	}

	// Specify a relative distribution of the sailors.
	public void ObeyOrder (float thrusters, float reloaders, float repairers, bool apply = true) {
		// Divide the crew amount to the three sectors, accounting for rounding missmatches
		var infos = new float[] { thrusters, reloaders, repairers };
		var sum = infos.Sum ();
		var distribution = infos.Select (info => Mathf.RoundToInt (info / sum * this.amount)).ToList();
		var diff = this.amount - distribution.Sum ();
		if (diff != 0) distribution [distribution.IndexOf (distribution.Max())] += diff;	// ... so that it sums up the "crew"

		ObeyOrder (distribution[0], distribution[1], distribution[2], apply);
	}

	// Specify an absolute distribution of the sailors.
	void ObeyOrder (int thrusters, int reloaders, int repairers, bool apply = true) {
		Thrusters = thrusters;
		Reloaders = reloaders;
		Repairers = repairers;

		if (!apply) return;

		Fabric.Answers.Answers.LogCustom ("Crew Order", new Dictionary<string, object> () {
			{ "Thrusters", Thrusters / amount },
			{ "Repairers", Repairers / amount },
			{ "Reloaders", Reloaders / amount }
		});

		steerman.sailorBoost         = thrusters * thrustPerSailor;
		ship.repairsPerSecond        = repairers * repairsPerSailor;
		backboard.reloadsPerSecond   = reloaders * reloadsPerSailor;
		steuerboard.reloadsPerSecond = reloaders * reloadsPerSailor;

		ship.RingBell (2);

		OnOrder.Invoke();
	}



	#endregion
}
