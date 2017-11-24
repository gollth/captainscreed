using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;
using System.Collections.Generic;

public class Treasure : MonoBehaviour {

	#region Types
	public enum LiftType { 
		Immediate,
		Reach,
		Destroy,
		Ram
	}
	#endregion

	#region Statics
	const string TREASURE_KEY = "treasure-{0}-{1}";
	static List<int> AllInThisLevel = new List<int>();
	static readonly Dictionary<string, int> total = new Dictionary<string, int> {
		{ "A" , 0 },
		{ "B" , 0 },
		{ "C" , 2 }
	};
	public static int Total (string level) {
		if (!total.ContainsKey(level)) return -1;
		else return total[level];
	}
	public static IEnumerable<int> Lifted (string level) { 
		for (int x = 1; x <= total[level]; x++) {
			if (!PlayerPrefs.HasKey(string.Format(TREASURE_KEY, level, x))) continue;
			var alreadylifted = PlayerPrefs.GetInt(string.Format(TREASURE_KEY, level, x));
			var result = AllInThisLevel.Find(available => available == alreadylifted);
			yield return result;
		}
		yield break;
	}
	protected static void Lift (string level, int treasureID) { 

		PlayerPrefs.SetInt(string.Format(TREASURE_KEY, level, AllInThisLevel.IndexOf(treasureID)+1), treasureID);
		PlayerPrefs.Save ();
	}
	#endregion

	#region Tweakables
	[Tooltip("The type of quest which has to fulfilled to receive this treasure")]
	public LiftType quest;
	[Tooltip ("The Value of this treasure in coins")]
	public int amount = 10;
	public AudioClip discoverSound;
	[Range(0,2), Tooltip ("The Explosion impulse spreading the coins out")]
	public float spread = 1;
	[Range (0,1), Tooltip ("Randomizes the spawn position of the coins")]
	public float randomness = 1f;
	#endregion

	#region Events
	public event System.Action<int, LiftType> OnDiscover;
	#endregion

	#region References
	private GameObject coin;
	private Transform particles;
	private bool discovered;
	private int? pending = null;
	#endregion

	#region Methods
	void OnEnable () {
		coin = Resources.Load ("UI/coin") as GameObject;
		particles = GameObject.FindGameObjectWithTag ("Particles").transform;
		SceneManager.sceneLoaded += OnLevelLoad;
		Quest.OnAllFinished += OnFulfilled;
		if (quest.Equals (LiftType.Immediate)) Discover ();
	}

	void Start() {
		if (GetComponent<AICaptain>() == null) AllInThisLevel.Add(this.GetInstanceID());
	}

	void OnDisable() {
		//Quest.OnAllFinished -= OnFulfilled;	// not do this, since OnDisable is called, when Treasure is destroyed
		SceneManager.sceneLoaded -= OnLevelLoad;

	}

	void Discover () {
		if (discovered) return;	// ... already discovered
		if (discoverSound != null) AudioSource.PlayClipAtPoint (discoverSound, this.transform.position);

		var wake = transform.GetComponentInChildren<ParticleSystem>();
		if (wake != null) {
			var scale = wake.transform.localScale;
			wake.transform.SetParent(particles, worldPositionStays: true);
			wake.transform.localScale = scale;
			wake.Pause();
		}

		// Create x coins pointing radially out
		var strength = spread;
		for (var i = 0; i < amount; i++) {
			var c = Instantiate (
				coin, 
				this.transform.position, 
				Quaternion.Euler (0, 0, (i + .5f* Random.Range (-randomness,randomness)) * 360f / amount)) 
				as GameObject;
			c.transform.parent = particles;
			var impulse = Random.Range (strength * (1 - randomness), strength) * Vector2.up;
			c.GetComponent<Rigidbody2D> ().AddRelativeForce (impulse, ForceMode2D.Impulse);
		}
		discovered = true;	// means that treasure cannot be discovered again
		if (GetComponent<AICaptain>() == null){ print("Pending " + name); pending = this.GetInstanceID(); }	// Findable treasure are those, which do not drive actively ;)
		if (OnDiscover != null) OnDiscover(amount, quest);

	}

	#endregion


	#region Callbacks
	void OnDestroy () {
		if (Level.Shutdown || SceneChanger.ChangingLevel) return;
		if (!quest.Equals (LiftType.Destroy)) return;
		Discover ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (!quest.Equals (LiftType.Ram)) return;
		if (!collision.collider.CompareTag ("Player") && !collision.collider.CompareTag("Cannonball")) return;
		this.Discover ();
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (!quest.Equals (LiftType.Reach)) return;
		if (!other.CompareTag ("Player") && !other.CompareTag("Cannonball")) return;
		this.Discover ();
		Destroy (this.gameObject);
	}

	void OnFulfilled(List<Quest> quests) {
		if (pending.HasValue) Lift(SceneManager.GetActiveScene().name, pending.Value);
	}

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		AllInThisLevel.Clear();
	}
	#endregion
}
