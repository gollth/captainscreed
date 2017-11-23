using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

	[SerializeField] Vector2 jitterStrength = new Vector2 (.017f, .01f);
	[SerializeField] Vector2 jitterFrequency= new Vector2 (.7646672f, 1f);
	[SerializeField] float   jitterRotation = 10;
	[SerializeField] bool gui = false;

	float seed;
	Vector3 originalPosition;
	Quaternion originalRotation;
	Rigidbody2D body;

	// Not OnEnable for Bubble support
	void Start () {
		seed = Random.Range(-1000, 1000);
		body = GetComponent<Rigidbody2D>();
		if (body == null) {
			originalPosition = transform.localPosition;
			originalRotation = transform.localRotation;
		} else {
			originalPosition = transform.position;
			originalRotation = transform.rotation;
		}
	}

	void Update () {
		if (body != null) return;
		var time = seed + Time.unscaledTime;
		var targetPosition = originalPosition + (Vector3)(
			jitterStrength.ScaleEach(gui ? Util.ScreenSize : Vector2.one)
						  .ScaleEach(Util.Sin (jitterFrequency * time)));
		var targetOrientation = originalRotation * Quaternion.AngleAxis (
			jitterRotation * Mathf.Sin (jitterFrequency.Mean () * time),
			Vector3.forward
		);
		transform.localPosition = targetPosition;
		transform.localRotation = targetOrientation;
	}

	void FixedUpdate () {
		if (body == null) return;
		var time = seed + Time.unscaledTime;
		body.velocity = jitterStrength.ScaleEach(gui ? Util.ScreenSize : Vector2.one)
									  .ScaleEach(Util.Sin (jitterFrequency * time));
		body.angularVelocity = jitterRotation * Mathf.Sin (jitterFrequency.Mean () * time);
	}
}
