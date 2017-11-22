using UnityEngine;
using UnityEngine.UI;
//using RectTransformExtensions;


using System.Collections;

public class ReloadIndicator : MonoBehaviour {

	public LocaleStringSetter text;
	public bool right;

	private Cannons cannons;
	private Button button;
	private RectMask2D mask;
	private Ship ship;

	// Use this for initialization
	void Awake () {
		mask = GetComponentInChildren<RectMask2D> ();
		button = GetComponentInChildren<Button> ();
		var player = GameObject.FindGameObjectWithTag ("Player");
		if (!right) cannons = Cannons.OnBackboardFrom (player);
		else      cannons = Cannons.OnSteuerboardFrom (player);

		ship = player.GetComponent<Ship> ();
		// Initialize mask
		var targetAnchorMax = mask.rectTransform.anchorMax;
		targetAnchorMax.y = cannons.ReadyRatio;
		mask.rectTransform.anchorMax = targetAnchorMax;

	}

    void Start() { SetText(); }

	void OnEnable () {  
		cannons.OnFired += SetText;
		cannons.OnReload += SetText;
		ship.OnDestroyed += DisableButton;
		
	}
	void OnDisable () {
		cannons.OnFired -= SetText;
		cannons.OnReload -= SetText;
		if (ship != null) ship.OnDestroyed -= DisableButton;
	}


    void SetText() { text.SetValues (cannons.Ready, cannons.amount); }
	void Update () {
		// Lerp the offset
		var targetAnchorMax = mask.rectTransform.anchorMax;
		targetAnchorMax.y = cannons.ReadyRatio;
		mask.rectTransform.anchorMax = Vector2.Lerp (mask.rectTransform.anchorMax, targetAnchorMax, 2 * Time.deltaTime);
	}
		

	void DisableButton () {
		this.gameObject.SetActive (false);
		button.enabled = false;
	}
}
