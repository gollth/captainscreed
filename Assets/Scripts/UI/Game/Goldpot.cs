using UnityEngine;
using System.Collections;

public class Goldpot : MonoBehaviour {

	#region Statics
	public const string KEY_GOLD = "gold";
	public static int Stowage { 
		get { return PlayerPrefs.GetInt (KEY_GOLD, 0); }
		private set { PlayerPrefs.SetInt (KEY_GOLD, value); /*PlayerPrefs.Save ();*/ }
	}
	public static void Buy (Price price) {
		Stowage -= price.Value;
		PlayerPrefs.Save ();
	}
	#endregion


	#region Properties
	public bool TransferFinished { get; private set; }
	#endregion

	#region Tweakables
	public LocaleStringSetter stowageUI;
	public LocaleStringSetter holdUI;
	[Tooltip ("The time it takes to transform one coin, e.g. from the hold into the stowage"), Range (0,1)]
	public float transferCoinDelay = 0.2f;
	#endregion

	#region References
	private Ship ship;
	private Treasure hold;
	private GameObject HoldUIContainer { get { return holdUI.transform.parent.gameObject; } }
	#endregion


	#region Methods
	void Start () {
		stowageUI.SetValues (Stowage);
		holdUI.SetValues (0);
		HoldUIContainer.SetActive (false);
	}

	void OnEnable () {
		var player = GameObject.FindGameObjectWithTag ("Player");
		ship = player.GetComponent<Ship> ();
		ship.OnDestroyed += LooseWholeHold;

		hold = player.GetComponent<Treasure> ();
	}

	void OnDisable () {
		if (ship != null) ship.OnDestroyed -= LooseWholeHold;
	}

	// Puts a coin from the battlefield into the ship's hold.
	public void StoreCoinInHold () {
		HoldUIContainer.SetActive (true);
		hold.amount ++;
		holdUI.SetValues (hold.amount);
	}

	// Secures the content of the ship's hold and stows it safely
	public void StoreHoldInStowage () {
		StartCoroutine (TransferCoins (-1, +1, until:() => hold.amount == 0, callback:() => {
			HoldUIContainer.SetActive (false);
			PlayerPrefs.Save ();
		}));
	}

	// Removes all coins currently stored in the ship's hold
	public void LooseWholeHold () {
		holdUI.SetValues (0);
		HoldUIContainer.SetActive (false);
		TransferFinished = true;
	}


	// This handles the counting animation of coins from one place to another
	IEnumerator TransferCoins (int holdIncrement, int stowageIncrement, System.Func<bool> until, System.Action callback=null) {
		TransferFinished = false;
		while (!until ()) {
			if (holdIncrement != 0){hold.amount += holdIncrement;    holdUI.SetValues (hold.amount); }
			if (stowageIncrement != 0){ Stowage += stowageIncrement; stowageUI.SetValues (Stowage); }
			yield return new WaitForSeconds (transferCoinDelay);
		}
		TransferFinished = true;
		if (callback != null) callback();
	}

	#endregion
}
