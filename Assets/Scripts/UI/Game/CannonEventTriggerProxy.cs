using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof (EventTrigger))]
public class CannonEventTriggerProxy : MonoBehaviour {

	private Cannons backboard, steuerboard;
	
	void Start () {
		
		var player = GameObject.FindGameObjectWithTag("Player");
		backboard = Cannons.OnBackboardFrom (player);
		steuerboard = Cannons.OnSteuerboardFrom (player);
		
	}
	
	
	public bool BackboardFiring { 
		get { return backboard.Fire; }
		set { backboard.Fire = value; }
	}
	
	public bool SteuerboardFiring {
		get { return steuerboard.Fire; }
		set { steuerboard.Fire = value; }
	}
}
