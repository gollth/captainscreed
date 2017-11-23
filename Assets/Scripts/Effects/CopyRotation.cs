using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CopyRotation : MonoBehaviour {

	[Tooltip ("Leave empty to use Camera.main")]
	public Transform target;
	public bool invert = false;
	[Range(-180, 180)] public float offset = 0;

	void OnEnable () {
		if (target == null) target = Camera.main.transform;
	}

	// Update is called once per frame
	void Update () {
		var rotation = invert ? Quaternion.Inverse (target.rotation) : target.rotation;
		this.transform.rotation = Quaternion.Euler (0,0, offset) * rotation;
	}
}
