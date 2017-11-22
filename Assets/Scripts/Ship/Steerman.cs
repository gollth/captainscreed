using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Steerman : MonoBehaviour {

	[Range(0, 90)]
	public float maxLuvAngle = 30;
	[Tooltip("This describes the relative Speed (y) over the luv angle between 0 and 180° (symmetric in both directions")]
	public AnimationCurve speedPerLuvAngle = AnimationCurve.Linear(0,0,180,1);
	[Range (0, 1)]	public float maneuverability = 0.5f;
	[Range(0, 1)]	public float maxSpeed = .5f;
	[Range(0, 1)] 	public float sailorBoost = 0;

	public bool InWind { get { return Mathf.Abs (Util.Mod180(GetLuvAngle ())) < maxLuvAngle; } }

	public float Rudder { get; set; }
	public float Heading { get { return -r.rotation; } }

	Wind wind;
	Rigidbody2D r;
	Ship ship;


	// Use this for initialization
	void Start() {
		wind = GameObject.FindGameObjectWithTag ("Wind").GetComponent<Wind> ();
		r = GetComponent<Rigidbody2D> ();
		ship = GetComponent<Ship> ();
	}
	void OnEnable()  { SceneManager.sceneLoaded += OnLevelLoad;	}
	void OnDisable() { SceneManager.sceneLoaded -= OnLevelLoad;	}

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		wind = GameObject.FindGameObjectWithTag ("Wind").GetComponent<Wind> ();
	}

	// 0 means in wind, 180 means wind from behind
	public float GetLuvAngle () {
		if (wind == null) return 0;
		return Util.Mod360(-(Quaternion.Inverse (transform.rotation) 
			* Quaternion.Euler(0,0, -wind.Direction)).eulerAngles.z -180 );
	}

	public float Speed { get; private set; }


	// Update is called once per frame
	void FixedUpdate () {
		var luv = Mathf.Abs (Util.Mod180 (GetLuvAngle ()));
		var targetSpeed = speedPerLuvAngle.Evaluate(luv)  * wind.Strength * maxSpeed;
		if (ship != null) 	targetSpeed *= ship.SpeedForCondition;

		Speed = Mathf.Lerp (Speed, targetSpeed  * (1 + sailorBoost), 1f / r.mass * Time.deltaTime);

		r.velocity = transform.up * Speed;
		r.angularVelocity = -Rudder * maneuverability * (1 + sailorBoost);	// Rudder inverts, because at the rear of the ship
	}
}
