using UnityEngine;

public class Statistics : MonoBehaviour {

	
	#region Properties
	public float Accuracy { get { if (shotsFired == 0) return 0; else return ((float)shotsHit) / shotsFired;}}
	public int ShipsSunk { get; private set;}
	public int TotalEnemyShipCount { get; private set; }
	#endregion

	#region References
	private int shotsFired;
	private int shotsHit;
	private GameObject player;
	#endregion
	
	#region Methods
	void OnEnable () { 
		Port.OnSpawn += OnEnemySpawned; 
		TotalEnemyShipCount = Port.QueueSize;
		ResourceLoader.WaitFor("Player", player => {
			foreach (var cannons in player.GetComponentsInChildren<Cannons>())
				cannons.OnFired += OnPlayerFired;
		});

	}
	void OnDisable() { 
		Port.OnSpawn -= OnEnemySpawned;
        if (player == null) return;
		foreach (var cannons in player.GetComponentsInChildren<Cannons>())
			cannons.OnFired -= OnPlayerFired;
		
	}

	void OnPlayerFired () { shotsFired++; }
	void OnEnemySpawned (Port port, Ship enemy) {
		enemy.OnDamage += (offender, damage) => {
			var ship = offender.GetComponentInParent<Ship>();
			if (ship == null) { Debug.LogWarning("[Statistics] Enemy " + enemy + " took damage from non-ship " + offender.name); return; }
			if (!ship.CompareTag("Player")) return;
            shotsHit++;
		};
		enemy.OnDestroyed += () => ShipsSunk++;
	}
	#endregion
}
