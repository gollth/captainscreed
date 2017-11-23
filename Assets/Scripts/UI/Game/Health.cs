using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (RectTransform))]
public class Health : MonoBehaviour {

	public LocaleStringSetter number;

	Ship player;
	RectMask2D mask;
	float health;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Ship> ();	
		health = player.RelativeHealth;
		mask = GetComponentInChildren<RectMask2D> ();
		SetMaskAnchorMax(health);

	}

	void SetMaskAnchorMax(float y) {
		var a = mask.rectTransform.anchorMax;
		a.y = y;
		mask.rectTransform.anchorMax = a;
	}

	void Update () {

		var targetHeath = player.RelativeHealth;
		health = Mathf.Lerp (health, targetHeath, 2 * Time.deltaTime);

		// Update UIs
		SetMaskAnchorMax(health);
		//mask.rectTransform.anchorMax = Vector2.Lerp (mask.rectTransform.anchorMax, targetAnchorMax, 2 * Time.deltaTime);

		number.SetValues (player.health);


	}
}
