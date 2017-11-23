using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent (typeof (LineRenderer))]
public class Route : MonoBehaviour {

	[SerializeField] Transform[] checks;

	LineRenderer line;

	void OnEnable() {
		line = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update () {
		line.SetPosition(0, this.transform.position);
		for (int i = 0; i < checks.Length; i++) line.SetPosition(i+1, checks[i].position);
	}
}
