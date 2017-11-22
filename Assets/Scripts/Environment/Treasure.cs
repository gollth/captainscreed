using UnityEngine;

using System.Collections;

public class Treasure : MonoBehaviour {

	public enum Quest { 
		Immediate,
		Reach,
		Destroy,
		Ram
	}
	#region Tweakables
	[Tooltip("The type of quest which has to fulfilled to receive this treasure")]
	public Quest quest;
	[Tooltip ("The Value of this treasure in coins")]
	public int amount = 10;
	public AudioClip discoverSound;
	[Range(0,2), Tooltip ("The Explosion impulse spreading the coins out")]
	public float spread = 1;
	[Range (0,1), Tooltip ("Randomizes the spawn position of the coins")]
	public float randomness = 1f;
	#endregion

	#region Events
	public event System.Action<int, Quest> OnDiscover;
	#endregion

	#region References
	private GameObject coin;
	private Transform particles;
	private bool discovered;
	#endregion

	#region Methods
	void OnEnable () {
		coin = Resources.Load ("UI/coin") as GameObject;
		particles = GameObject.FindGameObjectWithTag ("Particles").transform;
		if (quest.Equals (Quest.Immediate)) Discover ();
	}

	void Discover () {
		if (discovered) return;	// ... already discovered
		if (discoverSound != null) AudioSource.PlayClipAtPoint (discoverSound, this.transform.position);
		
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
		if (OnDiscover != null) OnDiscover(amount, quest);

	}

	#endregion


	#region Callbacks
	void OnDestroy () {
		if (Level.Shutdown || SceneChanger.ChangingLevel) return;
		if (!quest.Equals (Quest.Destroy)) return;
		Discover ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (!quest.Equals (Quest.Ram)) return;
		if (!collision.collider.CompareTag ("Player")) return;
		this.Discover ();
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (!quest.Equals (Quest.Reach)) return;
		if (!other.CompareTag ("Player")) return;
		this.Discover ();
		Destroy (this.gameObject);
	}
	#endregion
}
