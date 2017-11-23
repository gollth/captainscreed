using UnityEngine;

using System;
using System.Linq;
using System.Collections;

public class Port : MonoBehaviour {


	#region Statics
	public static event Action<Port, Ship> OnSpawn;
	public static int QueueSize { get; private set; }
	public static void ClearQueue () { QueueSize = 0; }
	#endregion

	#region Tweakables
	[Tooltip("The enemy to spawn here")]
	public GameObject enemy;
	[Range(0,180), Tooltip("The time to wait to spawn, in seconds")]
	public int delay = 0;
	[Tooltip ("This Port will not spawn the enemy until the enemies from all these ports have been destroyed")]
	public Port[] waitUntilSunk;

	[Range(0,30), Tooltip("The radius of the area, which have to be invisible to the main camera, until an enemy is spawned")]
	public float waitUntilAreaOutOfView = 5;
	public string bubbleOnSpawn = string.Empty;
	#endregion

	#region References
	bool spawned = false;
	int relevantShipsSunk;
	#endregion
	
	#region Methods
	void Awake () { QueueSize++; }

	// Use this for initialization
	void Start() {
		if (waitUntilSunk.Length == 0){ StartCoroutine (Spawn (delay)); return; }
	}

	void OnEnable () { OnSpawn += OnAnyPortSpawned; }
	void OnDisable () {OnSpawn -= OnAnyPortSpawned;}
	void OnAnyPortSpawned (Port port, Ship ship) {
		if (waitUntilSunk.Contains (port)) {
			ship.OnDestroyed += () => {
				relevantShipsSunk++;
				if (relevantShipsSunk == waitUntilSunk.Length) {
					StartCoroutine (Spawn (delay));
				}
			};
		}
	}
	
	void OnDrawGizmos () {
		if (spawned) return;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (this.transform.position, waitUntilAreaOutOfView);
	}

	IEnumerator Spawn (float d) {
		if (Level.Shutdown) yield break;
		
		yield return new WaitForSeconds (d);

		Vector3 direction;
		Vector3 mostVisiblePoint;
		do {
			direction = Camera.main.transform.position - this.transform.position;
			mostVisiblePoint = this.transform.position + direction.normalized * waitUntilAreaOutOfView;
			yield return new WaitForEndOfFrame ();
		} while (Util.IsVisibleToCamera (mostVisiblePoint));

		
		QueueSize--;
		var e = Instantiate (enemy, this.transform.position, this.transform.rotation) as GameObject;
		Indicator.CreateFor(e.transform, friendly: false);

		var player = GameObject.FindGameObjectWithTag ("Player");
		var course = Util.Direction2ZAngle (player.transform.InverseTransformPoint (e.transform.position));
		if (!string.IsNullOrEmpty (bubbleOnSpawn))
			Bubble.MessageByFirstOfficer (bubbleOnSpawn)
				.GetComponentInChildren<LocaleStringSetter> ()
				.SetValues (Util.ZAngle2Clock (course));
		
		if (OnSpawn != null) OnSpawn (this, e.GetComponent<Ship>());
		spawned = true;
	}
	#endregion
}
