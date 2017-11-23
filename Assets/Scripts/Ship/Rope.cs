using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
public class Rope : MonoBehaviour {

	[SerializeField] Transform clamp;
	[SerializeField, Range(0,1)] float scale = 0.05f;

	LineRenderer line;

	void OnEnable() {
		line = GetComponent<LineRenderer> ();
	}

	void Update () {
		if (clamp == null) return;
		line.useWorldSpace = true;
		line.SetPosition (0, clamp.position);
		line.SetPosition (1, this.transform.position);
		line.widthMultiplier = scale * transform.root.localScale.Mean ();
	}
}
