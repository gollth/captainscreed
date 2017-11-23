using UnityEngine;
using System.Collections;

public class Price : MonoBehaviour {

	[Tooltip("The thing to upgrade, e.g. crew, cannons, ship, or rigg")]
	public string type;
	[Tooltip ("Specifies the price (y) over the amount of payments (x)")]
	public AnimationCurve market = AnimationCurve.Linear (0,10,1,10);

	/// The amount, of how often this price has been paid
	public int Payments { get; private set; }
	// The current value of the price
	public int Value { get { return Mathf.RoundToInt (market.Evaluate (Payments));} }
	// If the coins the player owns is enough to pay this price
	public bool IsAffordable { get { return Value <= Goldpot.Stowage; } }


	private LocaleStringSetter setter;

	// Use this for initialization
	void Start () {
		Payments = PortlandShipYard.GetUpgradeCount (type);
		setter = GetComponentInChildren<LocaleStringSetter>();
		setter.SetValues (this.Value);
	}

	// Debug method, for clearing playerprefs
	public void ResetPayments () { Payments = 0; }

	public void Pay () {
		if (!IsAffordable) {
			Debug.LogWarningFormat ("Price.Pay() has been called on {0} " +
				"but the player acutally can't affort it (stowage {1} < price {2})", 
				transform.parent.name, Goldpot.Stowage, this.Value);
			return;
		}
		Goldpot.Buy (this);
		Payments++;
		setter.SetValues (this.Value);
	}
}
