using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Cannons : MonoBehaviour {
	#region Statics
	public static Cannons OnBackboardFrom (GameObject g) {
		return From (g, -g.transform.right);
	}
	public static Cannons OnSteuerboardFrom (GameObject g) {
		return From (g, g.transform.right);
	}
	public static Cannons From (GameObject g, bool right) {
		return From (g, (right ? 1 : -1) * g.transform.right);
	}
	private static Cannons From (GameObject g, Vector3 direction) {
		foreach (var cannons in g.GetComponentsInChildren<Cannons>())
			if (Vector3.Dot (cannons.transform.up, -direction) < 0.05f)
				return cannons;	// is pointing to left of g

		return null;	// Cannot find any cannons pointing in that direction on g
	}
	#endregion

	#region Tweakables
	[SerializeField] Cannons opposite;
	public GameObject cannonball;

	public float broadside = 1;
	public Vector2 spacing = new Vector2 (.2f, .08f);
	[Range(0,50)]
	public float frequency = 10;	
	[Range(0,10)]
	public float firePower = 3;		// Speed of the cannonballs

	[Range(0,100)]
	public int amount = 10;
	[Range(0,100)]
	public float reloadsPerSecond = 1/3f;
	#endregion

	#region Properties
	public int Ready { get; private set; }
	public float ReadyRatio { get { return (float)Ready / amount; } }
	public bool AllReady { get { return Ready == amount; } }
	public bool Fire { 
		get { return firing; }
		set {
			firing = value;
			if (firing) StartCoroutine (FireCannons ());
			else 	  StopAllCoroutines ();
		}
	}
	// a.k.a. fire till empty
	public void FullBroadside () { Fire = true; stopOnEmpty = true; }
	#endregion

	#region Events
	public event System.Action OnReload;
	public event System.Action OnFired;
	#endregion

	#region References
	bool firing = false;
	float timer = 0;		 // for reloading 
	GameObject particles;	 // parent holder
	bool stopOnEmpty;
	List<GameObject> cannons;
	#endregion

	#region Methods
	public void Recreate() {
		this.enabled = false;
		this.enabled = true;
	}

	void OnEnable()  { 
		SceneManager.sceneLoaded += OnLevelLoad;	
		particles = GameObject.FindGameObjectWithTag ("Particles");
		cannons = new List<GameObject> ();
		foreach (var location in PossibleCannonLocations()) {
			var go = Instantiate (Resources.Load ("Ships/cannon"), location.Value, transform.rotation, parent: transform) as GameObject;
			if (location.Key > 1) {
				var sprite = go.GetComponentInChildren<SpriteRenderer> ();
				sprite.sortingLayerName = "Background";
				sprite.sortingOrder = -location.Key;
			}
			cannons.Add (go);
		}

	}
	void Start() {
		// Now the "amount" parameter has been set by ShipYard
		Ready = amount;
		if (OnReload != null) OnReload();
	}

	void OnDisable() { 
		SceneManager.sceneLoaded -= OnLevelLoad;	
		foreach (var cannon in cannons) Destroy (cannon);
	}

	void Update () {
		// Either firing or reloading but not both ...
		if (firing && Ready > 0) return;

		// Only need to reload, if not yet done...
		if (Ready >= amount) return;
		if (reloadsPerSecond == 0) return;

		timer += Time.deltaTime;

		// Increase the amount of loaded cannons by the rate for this frame
		var reloadSpeed = reloadsPerSecond;
		if (!opposite.AllReady)	reloadSpeed /= 2;
		int loadedThisFrame = Mathf.FloorToInt(timer * reloadSpeed);

		Ready += loadedThisFrame;
		Ready = Mathf.Clamp (Ready, 0, amount);

		// "Reduce" the timer
		if (loadedThisFrame > 0) {
			timer -= 1f / reloadSpeed;

			// Update Editor if applicable
			if (OnReload != null) OnReload ();
		}

	}


	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		if (scene.buildIndex == 0) return;
		particles = GameObject.FindGameObjectWithTag ("Particles");
	}



	IEnumerable<KeyValuePair<int, Vector3>> PossibleCannonLocations() {
		var a = transform.position + transform.right * transform.lossyScale.x * broadside / 2 ;
		var b = transform.position - transform.right * transform.lossyScale.x * broadside / 2;

		var rows = Mathf.CeilToInt(amount / 10f);
		for(int r = 0; r < rows; r++) {
			var m = (int) (amount / (rows));
			for (int i = 0; i < m; i++) {
				var index = (i + spacing.x * r) / (m - 1);
				var offset = -transform.up * r * spacing.y;
				yield return new KeyValuePair<int, Vector3>(r+1, Vector3.LerpUnclamped (a, b, index) + offset);
			}
		}
	}

	Vector3 RandomSpawnPoint () {
		return cannons [Random.Range (0, cannons.Count - 1)].transform.position;
	}

	IEnumerator FireCannons() {
		while (Fire) {
			while (Ready <= 0) yield return new WaitForEndOfFrame ();	// Wait until one cannon loaded again
			
			var ball = (GameObject) Instantiate (cannonball, RandomSpawnPoint (), transform.rotation);
			ball.transform.parent = particles.transform;
			var script = ball.GetComponent<Cannonball> ();
            script.shooter = this;
            script.flyspeed = firePower;
            Ready -= 1;
			if (OnFired != null) OnFired ();

			if (stopOnEmpty && Ready == 0) {
				stopOnEmpty = false;
				Fire = false;
			}

			yield return new WaitForSeconds (1f / frequency);
		}
	}


	void OnDrawGizmosSelected () {
		if (Application.isPlaying) return;
		var size = transform.lossyScale / 10;
		size.x *= 4;
		foreach (var location in PossibleCannonLocations()) {
			if (location.Key > 1) Gizmos.color = location.Key > 1 ? Color.grey : new Color (.2f, .2f, .2f);
			Gizmos.DrawCube (location.Value - transform.up * size.x / 2, size);
		}
	}
	#endregion
}
