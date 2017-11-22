using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Ship : MonoBehaviour {

	#region Tweakables
	[Range(0,1000)] public int health = ShipYard.INITIAL_HEALTH;
	[Range(0,1000)] public int maxHealth = ShipYard.INITIAL_HEALTH;
	[Range (0,100)] public float repairsPerSecond = 1f;
	public AnimationCurve speedOverDamage = AnimationCurve.Linear (0,1,1,0);
	public AudioClip bell;
	[Range(0,150)] public int crew = 5;
	#endregion


	#region References
	float timer;
	float size = 1;
	Collider2D coll;
	#endregion


	#region Properties
	public float RelativeHealth { get { return (float)health/maxHealth; }}
	public float SpeedForCondition { get { return speedOverDamage.Evaluate (1 - RelativeHealth); }}
	public bool Destroyed { get { return health <= 0; } }
	#endregion

	#region Events
	public event System.Action OnDestroyed;
	public event System.Action<GameObject,int> OnDamage;
	public event System.Action OnRepair;
	#endregion

	#region Methods
	void Start() { 
		coll = GetComponent<Collider2D> ();
		var sprite = GetComponent<SpriteRenderer> ();
		if (sprite != null)	size = sprite.bounds.extents.magnitude;
	}

	// Update is called once per frame
	void Update () {
		if (health >= maxHealth) return;
		if (repairsPerSecond == 0) return;
		timer += Time.deltaTime;


		var repairSpeed = repairsPerSecond;
		int repairedThisFrame = Mathf.FloorToInt (timer * repairSpeed);
		health += repairedThisFrame;
		health = Mathf.Clamp (health, 0, maxHealth);

		// "Reduce" timer again
		if (repairedThisFrame > 0) {
			timer -= 1f / repairSpeed;

			// Broadcast event
			if (OnRepair != null) OnRepair();
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		switch (collision.gameObject.tag) {
		case "Cannonball":
			var ball = collision.gameObject.GetComponent<Cannonball> ();
			GameObject offender = null;
			if (ball.shooter != null) offender = ball.shooter.gameObject;
			TakeDamage (offender, ball.damage);
			break;
		case "Island": case "Pirate": case "Player": 	// in case of any collision with another object
			var impact = Mathf.Abs (Vector2.Dot (collision.contacts [0].normal, transform.up));
			TakeDamage (collision.gameObject, Mathf.RoundToInt (.5f * maxHealth * impact));
			break;
		}
	}


	public void TakeDamage (GameObject offender, int damage) {
		health -= damage;
		if (Application.isMobilePlatform && this.CompareTag ("Player")) Handheld.Vibrate();
		health = Mathf.Clamp (health, 0, maxHealth);
		if (OnDamage != null) OnDamage (offender, damage);
		if (health <= 0) {
			health = 0;
			this.enabled = false; 	// dont call Update anymore...
			if (OnDestroyed != null) OnDestroyed ();

			coll.enabled = false;
			Explosion.Detonate (transform.position, size);
			var wake = transform.Find("wake");
			if (wake != null) {
				wake.parent = GameObject.FindGameObjectWithTag("Particles").transform;
				var particles = wake.GetComponent<ParticleSystem>().emission;
				particles.enabled = false;	// for performance
			}
			Destroy (this.gameObject);

		}
	}
	IEnumerator Ring (int amount, float delay) {
		for (int i = 0; i < amount; i++) {
			AudioSource.PlayClipAtPoint (bell, this.transform.position, 2);
			yield return new WaitForSeconds (delay);
		}
	}

	public void RingBell (int amount = 1, float delay = 0.2f) {
		StartCoroutine (Ring (amount, delay));
	}

	#endregion
}
