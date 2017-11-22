using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Sail : MonoBehaviour {

	public enum Kind { Main, Cluver }

	[SerializeField] Kind kind = Kind.Main;

	[Range(0,5)]
	public float inWindJitter = 2.5f;
	[SerializeField, Range(0, 30)] float maxCluverAngle = 10;
	[SerializeField, Range(0,1)] float damageThreshold = 0;
	[SerializeField] Sprite damagedSail;

	#region References
	SpriteRenderer render;
	Steerman steerman;
	Ship ship;
	AudioSource flapping;
	Vector3 scale;
	Sprite originalSail;
	#endregion

	// Use this for initialization
	void OnEnable () {
		if (damageThreshold == 0) damageThreshold = Random.Range(.2f, 1f);
		render = GetComponent<SpriteRenderer>();
		steerman = GetComponentInParent<Steerman> ();
		ship = GetComponentInParent<Ship>();
		flapping = GetComponent<AudioSource> ();
		scale = transform.localScale;
		originalSail = render.sprite;
	}


	float CalculateBestSailAngle () {
		float windAngle = steerman.GetLuvAngle();	// 0 means straight up front, 180 means exactely from behind

		if (windAngle > 180) windAngle -= 360;		// normalize to [-180 ... 180]
		if (windAngle <-180) windAngle += 360;

		if (-90 - steerman.maxLuvAngle < windAngle && windAngle <= 0) windAngle = -90 - steerman.maxLuvAngle;
		if (90 + steerman.maxLuvAngle > windAngle && windAngle > 0)  windAngle = 90 + steerman.maxLuvAngle;

		return windAngle;
	}

	// Update is called once per frame
	void Update () {

		if (ship.RelativeHealth >= damageThreshold && render.sprite != originalSail)
			render.sprite = originalSail;

		if (ship.RelativeHealth < damageThreshold && render.sprite != damagedSail)
			render.sprite = damagedSail;

		switch (kind) {
		case Kind.Main:
			var targetAngle = CalculateBestSailAngle ();
			transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.Euler (0, 0, -90 - targetAngle), 2 * Time.deltaTime);

			// Check if we are really hard in wind. 3° margin allows for "Am Wind" sailing without jittering sails ;)
			bool inWind = Mathf.Abs (Util.Mod180 (steerman.GetLuvAngle ())) < steerman.maxLuvAngle - 3;

			if (inWind)
				transform.localRotation = transform.localRotation * Quaternion.Euler (0, 0, Time.timeScale * Random.Range (-inWindJitter, inWindJitter));

			if (flapping == null)
				return;
			flapping.volume = inWind ? Time.timeScale : 0;
			break;
		
		case Kind.Cluver:
			var luv = Util.Mod180 (steerman.GetLuvAngle ());
			luv = Mathf.Clamp (luv, -maxCluverAngle, maxCluverAngle);

			transform.localRotation = Quaternion.Slerp (
				transform.localRotation, 
				Quaternion.Euler (0, 0, -luv), 
				2 * Time.deltaTime
			);
			transform.localScale = Vector3.Lerp(
				transform.localScale, 
				Vector3.Scale(scale, new Vector3((luv < 0 ? 1 : -1), 1, 1)), 
				2 * Time.deltaTime
			);
			break;	
		}
	}
}
