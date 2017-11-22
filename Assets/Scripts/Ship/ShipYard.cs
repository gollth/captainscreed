using UnityEngine;

[RequireComponent(typeof(ResourceLoader))]
public class ShipYard : MonoBehaviour {

	#region Statics
	public const string KEY_CREW   = "crew";
	public const string KEY_HEALTH = "ship";
	public const string KEY_CANNONS = "cannons";
	public const string KEY_RIGG = "rigg";
	
	public const int INITIAL_CREW    =  5;
	public const int INITIAL_HEALTH  = 50;
	public const int INITiAL_CANNONS = 10;
	public const int INITIAL_RIGG    =  1;

	public const int MAX_POSSIBLE_CREW    =  45;
	public const int MAX_POSSIBLE_HEALTH  = 300;
	public const int MAX_POSSIBLE_CANNONS =  30;
	public const int MAX_POSSIBLE_RIGG    =   5;

	public const float MANEUVERABILITY_PER_RIGG = 0.1f;

	public static int CurrentCrew {  get { return PlayerPrefs.GetInt (KEY_CREW,   INITIAL_CREW); } }
	public static int CurrentHealth {get { return PlayerPrefs.GetInt (KEY_HEALTH, INITIAL_HEALTH);}}
	public static int CurrentCannons { get {return PlayerPrefs.GetInt (KEY_CANNONS,INITiAL_CANNONS);}}
	public static int CurrentRigg { get { return PlayerPrefs.GetInt (KEY_RIGG, INITIAL_RIGG);}}

	#endregion
	
	#region Tweakables
	public GameObject[] ships;
	[SerializeField] bool allowCrew = true;
	#endregion

	protected int SizeIndex { get { return  Mathf.RoundToInt (Mathf.Lerp (0, ships.Length - 1, 
		((float)CurrentHealth - INITIAL_HEALTH) / (MAX_POSSIBLE_HEALTH - INITIAL_HEALTH))); }}

	ResourceLoader loader;

	#region Methods
	public virtual void Awake() {}

	public virtual void OnEnable () {
		loader = GetComponent<ResourceLoader> ();
		Construct();
	}
	
	
	protected GameObject updateParameters (GameObject player) {
		var sailors = CurrentCrew;
		var health  = CurrentHealth;
		var cannons = CurrentCannons;
		var sails   = CurrentRigg;

		var crew = player.GetComponent<Crew> ();
		var ship = player.GetComponent<Ship>();
		var steerman = player.GetComponent<Steerman>();
		var broadsides = player.GetComponentsInChildren<Cannons>();
		var rigg = player.GetComponentInChildren<Rigg> ();

		crew.amount = sailors;
		crew.thrustPerSailor = 1f / health;
		
		ship.health = health;
		ship.maxHealth = health;

		rigg.level = sails;


		// Distribute all available cannons on both sides 
		foreach (var c in broadsides) c.amount = cannons / 2;

		// Let each mismatch from sail level to ship size boost/slow the ship
		var shipsize = (float)health / 50;
		steerman.maxSpeed = 0.5f + (sails - shipsize) * .1f;

		return player;
	}
	public virtual GameObject Construct() {

		// Create player and get references ot required elements
		var player = loader.Load(ships[SizeIndex]);
		player.GetComponent<Crew>().enabled = allowCrew;

		// Apply the important modifications ...
		updateParameters(player);

		return player;
	}
	

	#endregion
	
}
