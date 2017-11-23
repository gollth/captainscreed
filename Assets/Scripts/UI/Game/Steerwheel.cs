using UnityEngine;
using UnityEngine.EventSystems;

using System.Linq;
using System.Collections.Generic;


[RequireComponent(typeof(Collider2D)), 
	RequireComponent(typeof(RectTransform)), 
	RequireComponent(typeof(Rigidbody2D))]
public class Steerwheel : MonoBehaviour { 

	#region Tweakables
	[Tooltip("The higher this value, the farther drags will turn the wheel")]
	public float sensitivity = 50;
	[Tooltip ("The higher this value, the more the wheel will keep its angular speed on release")]
	public float inertia = 10;
	[Range(0,360)]
	public float wheelLimit = 360;
	[Range(0,90)]
	public float rudderLimit = 90;
	[Range (0,5), Tooltip("The strength of the P-Controller, which pulls the steerwheel back into its default position")]
	public float autoMiddlePull = .005f;
	public Cushion right;
	public Cushion left;
	public Collider2D limit;
	#endregion

	#region References
	private Steerman steerman;
	private Ship ship;
	protected Cannons[] cannons;	// only for debug
	private Rigidbody2D wheel;
	private AudioSource creeking;

	private float wheelRotation;
	private float rotationOffset;
	private float touchStartRotation;
	private float targetRotation;
	private float torque;
	private bool activeSteering;
	private CircularBuffer<float> speedHistory;
	private float lastRotation;
	#endregion

	#region Properties
	public float Rudder { get { return wheel == null ? 0 : -wheelRotation * rudderLimit / wheelLimit; } }
	#endregion

	#region Methods
	void Awake () {
		steerman = GameObject.FindGameObjectWithTag ("Player").GetComponent<Steerman> ();
		wheel = GetComponent<Rigidbody2D> ();
		ship = steerman.GetComponent<Ship> ();
		cannons = ship.GetComponentsInChildren<Cannons> ();
		creeking = GetComponent<AudioSource> ();
		speedHistory = new CircularBuffer<float>(3);
		lastRotation = wheel.rotation - transform.root.eulerAngles.z;
		wheel.centerOfMass = Vector2.zero;
		resetMomentum ();
	}

	void OnEnable() { ship.OnDestroyed += Disable; rotationOffset = 0;  }
	void OnDisable() { if (ship != null) ship.OnDestroyed -= Disable; }
	void Disable() { 
		this.gameObject.SetActive (false);
		wheel.freezeRotation = true;
	}

	void Update () {
		steerman.Rudder = Rudder;
		creeking.volume = Mathf.Clamp01 (wheel.angularVelocity);	// If wheel stands still, disable creeking
	}

	void FixedUpdate () {
		wheelRotation = wheel.rotation - transform.root.eulerAngles.z + rotationOffset;
		if (wheelRotation > lastRotation + 180) {
			rotationOffset -= 360;
			wheelRotation -= 360;
		}
		if (wheelRotation < lastRotation - 180)  {
			rotationOffset += 360;
			wheelRotation += 360;
		}

		if (!activeSteering) wheel.AddTorque (-wheelRotation * autoMiddlePull);
		left.enabled = wheelRotation < -wheelLimit;
		right.enabled = wheelRotation > wheelLimit;
		limit.enabled = left.enabled || right.enabled;

		wheel.AddTorque (torque);

		speedHistory.Add ((wheelRotation - lastRotation) / Time.fixedDeltaTime);
		lastRotation = wheelRotation;

		#if UNITY_EDITOR
		//Simulate controls
		if (Input.GetKey (KeyCode.LeftArrow)) wheel.angularVelocity = +120;
		if (Input.GetKey (KeyCode.RightArrow)) wheel.angularVelocity =-120;

		if (Input.GetKeyDown (KeyCode.D)) cannons[1].Fire = true;
		if (Input.GetKeyUp (KeyCode.D)) cannons[1].Fire = false;
		if (Input.GetKeyDown (KeyCode.A)) cannons[0].Fire = true;
		if (Input.GetKeyUp (KeyCode.A)) cannons[0].Fire = false;

        if (Input.GetKeyDown(KeyCode.Space)) FindObjectOfType<Crownest>().Zoomed = true;
        if (Input.GetKeyUp(KeyCode.Space)) FindObjectOfType<Crownest>().Zoomed = false;
		#endif
    }

	void resetMomentum () {
		wheel.angularVelocity = 0;
	}

	float getInstantSpeed (PointerEventData touch) {
		var direction = touch.delta.ScaleEach (Util.ScreenSize.Inverse ());;
		var instantSpeed = Mathf.Sign (direction.x) * direction.magnitude * inertia * sensitivity;
		return -instantSpeed;
	}
	
	public void OnPointerDown (BaseEventData data) {
		resetMomentum ();
		activeSteering = true;
	}

	
	
	public void OnDrag (BaseEventData data) {
		var touch = data as PointerEventData;

	
		// Saturate motion outside of limits
		if (Util.Outside (wheelRotation, -wheelLimit, wheelLimit)) {
			var offlimit = Mathf.Abs (wheelRotation) - wheelLimit;
			var movement = (touch.position - touch.pressPosition).ScaleEach(Util.ScreenSizePx.Inverse());
			torque = -movement.x / (offlimit / 10);
		} else {
			var movement = touch.delta.ScaleEach(Util.ScreenSize.Inverse());
			//wheel.angularVelocity = -Mathf.Clamp(movement.x * sensitivity, -360, +360);
			wheel.angularVelocity = -movement.x * sensitivity;
			torque = 0;
		}
		
		activeSteering = true;
	}

	public void OnPointerUp (BaseEventData data) {
		var speed = speedHistory.Average();
		speed = Mathf.Clamp (speed, -250, 250);
		wheel.angularVelocity = speed;
		
		activeSteering = false;
		torque = 0;

	}
	#endregion
}
