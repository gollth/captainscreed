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
	static List<string> AllInThisLevel = new List<string>();
	const string TREASURE_KEY = "treasure-{0}-{1}";
	static readonly Dictionary<string, int> total = new Dictionary<string, int> {
		{ "Title", 0 },
		{ "A" , 0 },
		{ "B" , 0 },
		{ "C" , 2 }
	};
	public static int Total (string level) {
		if (!total.ContainsKey(level)) return -1;
		else return total[level];
	}
	public static IEnumerable<string> Lifted (string level=null) {
		if (level == null) level = SceneManager.GetActiveScene().name; 
		for (int x = 1; x <= total[level]; x++) {
			if (!PlayerPrefs.HasKey(string.Format(TREASURE_KEY, level, x))) continue;
			yield return PlayerPrefs.GetString(string.Format(TREASURE_KEY, level, x));
		}
		yield break;
	}
	protected static void Lift (string level, string name) { 
		PlayerPrefs.SetString(string.Format(TREASURE_KEY, level, AllInThisLevel.IndexOf(name)+1), name);
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
	private string pending;
	#endregion

	#region Methods
	void OnEnable () {
		if (Lifted(SceneManager.GetActiveScene().name).Contains(this.name)) {
			this.gameObject.SetActive(false);	// we already found this treasure, thats why we ignore it
			return;
		}
		pending = null;
		coin = Resources.Load ("UI/coin") as GameObject;
		particles = GameObject.FindGameObjectWithTag ("Particles").transform;
		SceneManager.sceneLoaded += OnLevelLoad;
		Quest.OnAllFinished += OnFulfilled;
		if (quest.Equals (LiftType.Immediate)) Discover ();
	}

	void Start() {
		if (GetComponent<AICaptain>() != null) return;
		AllInThisLevel.Add(this.name);
		AllInThisLevel.Sort();
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
		if (GetComponent<AICaptain>() == null) pending = this.name;	// Findable treasure are those, which do not drive actively ;)
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
		if (pending != null) Lift(SceneManager.GetActiveScene().name, pending);
	}

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		AllInThisLevel.Clear();
	}
	#endregion
}
