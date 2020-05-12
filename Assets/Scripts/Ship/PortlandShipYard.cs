using UnityEngine;
using System.Collections.Generic;
using SmartLocalization;

public class PortlandShipYard : ShipYard {

	#region Statics
	public const int CREW_INCREMENT    =  5;
	public const int HEALTH_INCREMENT  = 25;
	public const int RIGG_INCREMENT    =  1;
	public const int CANNONS_INCREMENT =  4;

	public static int GetUpgradeCount(string key) {
		switch (key) {
		case KEY_CREW: 		return (CurrentCrew - INITIAL_CREW) / CREW_INCREMENT ;
		case KEY_HEALTH:	return (CurrentHealth - INITIAL_HEALTH) / HEALTH_INCREMENT;
		case KEY_CANNONS:	return (CurrentCannons - INITiAL_CANNONS) / CANNONS_INCREMENT;
		case KEY_RIGG:      return (CurrentRigg - INITIAL_RIGG) / RIGG_INCREMENT;
		default: 
			Debug.LogWarning ("Cannot find Upgrade count for key " + key + " since it doesn't exist");
			return -1;
		}
	}
	#endregion

	#region Tweakables
	public bool disableCrewMenu = false;
	public string messageOnShipUpgrade;
	#endregion

	#region Properties & References
	protected bool HasSizeChanged { get { return SizeIndex != lastSizeIndex; } }
	protected int lastSizeIndex;
	#endregion


	#region Methods
	public override void Awake () {
		lastSizeIndex = SizeIndex;
		base.Awake ();
	}


	public override GameObject Construct () {
		var ship = base.Construct ();

		if (disableCrewMenu) {
			var crew = ship.GetComponent<Crew> ();
			crew.spawnCrewMenuOnStart = false;
			crew.enabled = false;
		}

		if (HasSizeChanged) {
			if (!string.IsNullOrEmpty (messageOnShipUpgrade)) {
				var bubble = Bubble.MessageByFirstOfficer (messageOnShipUpgrade);
				var key = ship.name.Replace ("(Clone)", "");
				var shipName = LanguageManager.Instance.GetTextValue (key);
				bubble.GetComponentInChildren<LocaleStringSetter> ().SetValues (shipName);
			}
		}
		lastSizeIndex = SizeIndex;
		return ship;
	}


	GameObject Upgrade (GameObject player) {

		if (HasSizeChanged) {
			// need to reinstantiate, because size have changed...
			Destroy (player.gameObject);
			return Construct();

		} else return base.updateParameters(player);
	}

	public void OnCrewUpgrade(Price price) {
		var crew = CurrentCrew;
		crew += CREW_INCREMENT;
		PlayerPrefs.SetInt (KEY_CREW, crew);
		PlayerPrefs.Save();

		Upgrade (GameObject.FindGameObjectWithTag ("Player"));
		//Fabric.Answers.Answers.LogPurchase (price.Value, "EUR", true, "Crew", null, null, new Dictionary<string, object> () { {"Members", crew} });
		price.Pay();

	}

	public void OnShipUpgrade (Price price) {
		var health = CurrentHealth;
		health += HEALTH_INCREMENT;
		PlayerPrefs.SetInt (KEY_HEALTH, health);
		PlayerPrefs.Save();

		//Fabric.Answers.Answers.LogPurchase (price.Value, "EUR", true, "Crew", null, null, new Dictionary<string, object> () { {"Health", health} });
		
		price.Pay();
		Upgrade (GameObject.FindGameObjectWithTag ("Player"));
	}

	public void OnRiggUpgrade (Price price) {
		var rig = CurrentRigg;
		rig += RIGG_INCREMENT;
		PlayerPrefs.SetInt (KEY_RIGG, rig);
		PlayerPrefs.Save();

		var player = Upgrade (GameObject.FindGameObjectWithTag ("Player"));
		player.GetComponentInChildren<Rigg> ().level = rig;
		//Fabric.Answers.Answers.LogPurchase (price.Value, "EUR", true, "Crew", null, null, new Dictionary<string, object> () { {"Rigg", rig} });
		
		price.Pay();

	}

	public void OnCannonsUpgrade (Price price) {
		var cannons = CurrentCannons;
		cannons += CANNONS_INCREMENT;
		PlayerPrefs.SetInt (KEY_CANNONS, cannons);
		PlayerPrefs.Save();

		var player = Upgrade (GameObject.FindGameObjectWithTag ("Player"));
		foreach (var cannon in player.GetComponentsInChildren<Cannons>()) cannon.Recreate ();

		//Fabric.Answers.Answers.LogPurchase (price.Value, "EUR", true, "Crew", null, null, new Dictionary<string, object> () { {"Cannons", cannons} });
		
		price.Pay();

	}
	#endregion
}
