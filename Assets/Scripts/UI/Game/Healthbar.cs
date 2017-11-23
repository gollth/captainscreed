using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Healthbar : MonoBehaviour {

	[SerializeField] SpriteRenderer rect;
	[SerializeField] SpriteRenderer fill;

	Ship ship;
	float health;

	void Start() {
		ship = transform.GetComponentInParent<Ship>();
	}

	void Update () {
		health = Mathf.Lerp(health, ship.RelativeHealth, 3 * Time.deltaTime);
		var p = fill.transform.localPosition;
		p.x = Mathf.Lerp(-rect.size.x/2, 0, health);
		fill.transform.localPosition = p;
		fill.size = new Vector2(Mathf.Lerp(0, rect.size.x, health), rect.size.y);
	}
}
