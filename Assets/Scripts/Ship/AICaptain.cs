using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Steerman))]	// Captain is only as good as it's first offices ;)
public class AICaptain : MonoBehaviour {

	#region Statics
	private static List<GameObject> enemies = new List<GameObject>();
	public static int FleetSize { get { return enemies.Count; }}
	#endregion

	#region Tweakables
	[Range(0,10), Tooltip("Overall \"Chasiness\"")]
	public float P = 1.0f;
	[Range(0,90), Tooltip("If the broadside angle (left or right) are in line with the target direction below this angle, then fire")]
	public float fireBelow = 10;
	[Tooltip("If I keep aligned with the broadside, fire again after this amount of seconds"), Range (0,15)]
	public float cooldownTime = 5f;
	[Tooltip("If this amount of cannons have reloaded, fire again"), Range(0,100)]
	public float cooldownReload = 90;
	[Tooltip ("The higher this value, the more the ship will \"aim ahead\" to compensate the player's cruise speed")]
	public float aheadAiming = 0;
	[Tooltip ("The higher this value, the early the aicaptain will avoid forward islands"), Range(0,40)]
	public float islandSenseRadius = 10f;
	#endregion

	#region Properties
	public bool Turning { get; private set; }
	#endregion

	#region References
	private Cannons backboard;
	private Cannons steuerboard;

	private Steerman player;
	private Steerman steerman;

	private List<CircleCollider2D> targets;
	private CircleCollider2D closestTarget;

	private string originalName;
	private bool fired;
	private Coroutine cooldown;

	private bool chasing = true;
	private int layer;
	private RaycastHit2D sense;
	private CircularBuffer<RaycastHit2D> senses;
	private PlayerFollower follower;
	#endregion

	#region Events
	public static event Action<GameObject> OnEnemySpawned;
	public static event Action<GameObject> OnEnemyDied;
	#endregion

	#region Methods
	void Awake () {
		enemies.Add (this.gameObject);
		if (OnEnemySpawned != null) OnEnemySpawned(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Steerman>();
		steerman = GetComponent<Steerman> ();
		backboard = Cannons.OnBackboardFrom (this.gameObject);
		steuerboard = Cannons.OnSteuerboardFrom (this.gameObject);
		follower = Camera.main.GetComponent<PlayerFollower>();

		senses = new CircularBuffer<RaycastHit2D> (100);

		originalName = name;
		layer = LayerMask.GetMask ("Island");

		targets = new List<CircleCollider2D> ();
		foreach (var target in GameObject.FindGameObjectsWithTag ("Target")) {
			targets.Add (target.GetComponent<CircleCollider2D>());
		}
	}

	void OnDestroy () {
		enemies.Remove (this.gameObject);
		follower.Disengage(this.gameObject);
		if (Level.Shutdown) return;    // Don't call events, if engine shuts down
        if (OnEnemyDied != null) OnEnemyDied (this.gameObject);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (this.transform.position, islandSenseRadius);
	}

	Vector3 CalcClosestTarget () {
		Vector3 direction = new Vector3(float.MaxValue, float.MaxValue);
		foreach (var target in targets) {
			var d = target.transform.position - this.transform.position;
			if (d.magnitude < direction.magnitude) {
				direction = d;
				closestTarget = target;
			}
		}
		return direction;
	}

	bool TargetDestroyed () {
		foreach (var target in targets) if (target == null) return true;
		return false;
	}

	IEnumerator CooldownAndResetFire (bool rightside) {
		// Wait the specified time
		yield return new WaitForSeconds (cooldownTime);

		// Wait if all cannons have been reloaded
		var cannons = rightside ? steuerboard : backboard;
		yield return new WaitUntil (() => cannons.ReadyRatio * 100 >= cooldownReload);

		fired = false;
	}

	// Update is called once per frame
	void Update () {
		if (TargetDestroyed()) return;

		// Check if we are on collision course with an island, and avoid if necessary
		if (!AvoidingIslands()) return;

		Vector3 direction = CalcClosestTarget ();
		chasing = !closestTarget.OverlapPoint (this.transform.position);

		// Not yet in range ...
		if (chasing) Chase(direction); 

		// Close enough to turn and fire
		else if (BroadsideAligned()) Fire();



	}

	void FixedUpdate () {
		var sense = Physics2D.Raycast (
			this.transform.position, 	// from
			this.transform.up, 			// into
			islandSenseRadius, 			// maximal
			layer);						// only
		if (sense.collider == null) {
			senses.Clear ();
		} else senses.Add (sense);
	}

	bool AvoidingIslands () {
		Vector2 normal = Vector2.zero;
		foreach (var sense in senses) normal += sense.normal;

		if (normal.magnitude > 0) {
			Vector3 direction = normal.normalized;
			Util.DrawArrow (transform.position, direction * 5f, Color.black);
			SteerInto (direction, " (Avoiding)");
			return false;
		}
		return true;
	}

	void Chase (Vector3 direction) {
		fired = false;
		Util.DrawArrow (transform.position, direction * 5f, Color.red);
		SteerInto (direction, " (Chasing)");

	}

	void SteerInto (Vector3 direction, string state = "") {
		var course = Util.Direction2ZAngle (direction);
		var targetCourse = course;

		// Wind Check:
		if (steerman.InWind && !Turning) {
			// ... wir müssen "kreuzen", also wieder abfallen
			course = Mathf.Clamp (course, steerman.maxLuvAngle, 360 - steerman.maxLuvAngle);
			Util.DrawArrow (transform.position, course, .3f * direction.magnitude, Color.magenta);
		} 
		// Lohnt es sich schon auf den neuen Bug zu wenden?
		if (steerman.InWind && Mathf.Abs (targetCourse - steerman.Heading) > 2 * steerman.maxLuvAngle) {
			Turning = true;
			state = " (Turning)";
		}

		if (!steerman.InWind) Turning = false;

		// P-Controller to steer the rudder
		var error = -Mathf.DeltaAngle (course, steerman.Heading);
		steerman.Rudder = Mathf.Clamp (P * error, -90, 90);

		name = originalName + state;
	}

	bool BroadsideAligned () {
		
		//And check if ready to fire
		var target = player.transform.position;
		var distanceToTarget = this.transform.position - target;
		// Add a little offset in cruise direction of player
		target += player.transform.up * distanceToTarget.magnitude * aheadAiming;

		var aim = target - this.transform.position;		// direction to player
		var missalignment = Vector2.Angle (aim, transform.right);			// angle between left-right axis and direction to player 
	
		if (missalignment > 90)	missalignment -= 180;
		missalignment = Mathf.Abs (missalignment);

		Debug.DrawRay (transform.position, 0.5f * aim, Color.red);

		steerman.Rudder = P * missalignment;

		// Not yet / anymore aligned ...
		if (missalignment >= fireBelow) {
			name = originalName + " (Aligning)";
			follower.Engage(this.gameObject);
			// again ready to fire NOW!!!
			if (cooldown != null) StopCoroutine (cooldown);		// if running
			fired = false;					

		}

		return missalignment < fireBelow;
	}

	void Fire () {
		// Only trigger once...
		if (fired) return;

		name = originalName + " (Firing)";

		// Fire the corresponding cannons
		var playerRightOfMe = Vector3.Dot (player.transform.position - this.transform.position, this.transform.right) > 0;
		(playerRightOfMe ? steuerboard : backboard).FullBroadside();

		fired = true;

		cooldown = StartCoroutine (CooldownAndResetFire (playerRightOfMe));
	}
	#endregion
}
