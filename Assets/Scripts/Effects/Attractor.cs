using UnityEngine;
using System.Collections;

public class Attractor : MonoBehaviour {

	public string[] layers;
	public float range = 10;
	public float strength = 1;

	public bool quadratricFalloff = true;
	
	private int mask;

	// Use this for initialization
	void Start () {
		mask = LayerMask.GetMask (layers);
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (this.transform.position, range);
	}

	// Update is called once per frame
	void FixedUpdate () {
	
		foreach (var collider in Physics2D.OverlapCircleAll (this.transform.position, range, mask)) {
			if (collider.attachedRigidbody == null) continue;

			var direction = this.transform.position - collider.transform.position;
			direction.z = 0;
			//if (direction.magnitude <= 1.0f) continue;
			collider.attachedRigidbody.AddForce (
				direction.normalized * strength / Mathf.Max (
					quadratricFalloff ? direction.sqrMagnitude : direction.magnitude, 
					.0001f
				)
			);
		}

	}
}
