using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UpgradeMenu : MonoBehaviour {

	#region Tweakables
	public ShipYard shipyard;

	public LocaleStringSetter gold;
	public LocaleStringSetter crew;
	public LocaleStringSetter ship;
	public LocaleStringSetter rigg;
	public LocaleStringSetter cannons;

	public Button crewUpgrade;
	public Button shipUpgrade;
	public Button riggUpgrade;
	public Button cannonsUpgrade;

	#endregion


	#region References
	private Price crewPrice;
	private Price shipPrice;
	private Price riggPrice;
	private Price cannonsPrice;
	#endregion

	// Use this for initialization
	void Start () {
		crewPrice = crewUpgrade.transform.GetComponentInSibling <Price> ();
		shipPrice = shipUpgrade.transform.GetComponentInSibling <Price> ();
		riggPrice = riggUpgrade.transform.GetComponentInSibling <Price> ();
		cannonsPrice = cannonsUpgrade.transform.GetComponentInSibling<Price> ();

		UpdateNumber ();
	}


	public void UpdateNumber () {
		gold.SetValues (Goldpot.Stowage);

		crew.SetValues (ShipYard.CurrentCrew);
		crewUpgrade.interactable = ShipYard.CurrentCrew < ShipYard.MAX_POSSIBLE_CREW
										&& crewPrice.IsAffordable;
		ship.SetValues (ShipYard.CurrentHealth);
		shipUpgrade.interactable = ShipYard.CurrentHealth < ShipYard.MAX_POSSIBLE_HEALTH
										&& shipPrice.IsAffordable;
		rigg.SetValues (ShipYard.CurrentRigg);
		riggUpgrade.interactable = ShipYard.CurrentRigg < ShipYard.MAX_POSSIBLE_RIGG
										&& riggPrice.IsAffordable;
		cannons.SetValues (ShipYard.CurrentCannons);
		cannonsUpgrade.interactable = ShipYard.CurrentCannons < ShipYard.MAX_POSSIBLE_CANNONS
										&& cannonsPrice.IsAffordable;
	}
	
	public void ClearUpgrades () {
		PlayerPrefs.DeleteAll ();
		Destroy (GameObject.FindGameObjectWithTag ("Player"));
		UpdateNumber ();
		foreach (var price in FindObjectsOfType<Price>()) price.ResetPayments ();
		shipyard.Construct ();
	}

	public void DebugAddGold (int amount) {
		PlayerPrefs.SetInt (Goldpot.KEY_GOLD, Goldpot.Stowage + amount); 
		PlayerPrefs.Save ();
		this.UpdateNumber ();
	}
}
