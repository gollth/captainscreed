using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class PlayerFollower : MonoBehaviour {

	[Range(0,1)] public float movementSpeed = 1f;
	[Range(0,1)] public float rotationSpeed = .3f;
	[Range(0,5)] public float playerWeight = 2f;
	[Range(0,100)] public float maxFov = 45f;
	[Range(0,100)] public float minFov = 10;
	[Range(0,1)] public float border = .3f;

	public Util.UnityEventGameObject OnEngage = new Util.UnityEventGameObject();
	public Util.UnityEventGameObject OnDisengage = new Util.UnityEventGameObject();


	List<Transform> enemies;
	Camera cam;
	Ship player;
	Vector3 initialOffset;
	float initialFov;
	float fovspeed;

	void OnEnable () {
		enemies = new List<Transform>();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Ship>();
		player.OnDestroyed += Stop;
		cam = GetComponent<Camera>();
		initialOffset =  player.transform.position - this.transform.position;
		initialFov  = cam.orthographicSize;
		fovspeed = 0f;
	}
	
	
	void Disable () {
		enemies.Clear();
		if (player != null) player.OnDestroyed -= Stop;
	
	}
	
	void Stop () { this.enabled = false; }
	
	void Update () {
		// Find the average position of all visible ships (including ourselves)
		var position = player.transform.position * playerWeight;

		Transform farest = player.transform;
		var max = initialFov;
		foreach (var enemy in enemies) {
			position += enemy.position;
			var f = CalcEngagementFov(enemy.position);
			if (f <= max) continue;
			max = f;
			farest = enemy;
		}
		position /= enemies.Count + playerWeight;
		var fov = max;
		if (fov == 0) fov = initialFov;
		if (fov >= maxFov) { Disengage(farest.gameObject); fov = maxFov; }
		if (fov <  minFov) fov = minFov;

		this.cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, fov, ref fovspeed, 0.1f);
		this.transform.position = Vector3.Lerp (
			this.transform.position,
			position - cam.transform.TransformDirection(initialOffset),
			movementSpeed * Time.deltaTime
		);
		this.transform.rotation = Quaternion.Slerp (
			this.transform.rotation, 
			player.transform.rotation, 
			rotationSpeed * Time.deltaTime
		);
	}

	float CalcEngagementFov (Vector3 target) {
		var vector = target - cam.transform.position;
		vector.z = 0;
		var distance = vector.magnitude;
		return distance / cam.aspect * (1+border);
	}

	public void Engage (GameObject other) {
		if (!other.CompareTag("Pirate")) return;
		if (enemies.Contains(other.transform)) return;
		if (InEngagementDistance(other.transform)) return;
		enemies.Add(other.transform);
		OnEngage.Invoke(other.gameObject);
	}

	public bool InEngagementDistance(Transform target) { return CalcEngagementFov(target.position) > maxFov; }

	public void Disengage(GameObject other) {
		if (other == null) { enemies = enemies.Where(item => item != null).ToList(); return; }
		if (!other.CompareTag("Pirate")) return;
		enemies.Remove(other.transform);
		OnDisengage.Invoke(other.gameObject);
	}

}
