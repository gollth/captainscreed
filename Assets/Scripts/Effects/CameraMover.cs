using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour {

	[SerializeField] GoEaseType ease = GoEaseType.ExpoOut;
	[SerializeField] float duration = 1f;
	[SerializeField] Transform startWith;

	Stack<Transform> history = new Stack<Transform> ();

	void OnEnable() {
		if (startWith == null) return;
		transform.position = startWith.position;
		transform.rotation = startWith.rotation;
		history.Push (startWith);
	}
		
	public void Towards(Transform aim) {
		history.Push (aim);
		Go.to (transform, duration, config: new GoTweenConfig ()
			.position (aim.position)
			.rotation (aim.rotation)
			.setEaseType (ease)
		);
	}

	public void Undo() {
		if (history.Count == 1) return;
		history.Pop ();
		var aim = history.Peek();
		Go.to (transform, duration, config: new GoTweenConfig ()
			.position (aim.position)
			.rotation (aim.rotation)
			.setEaseType (ease)
		);
	}

	void Update() {
		if (Input.GetKeyUp (KeyCode.Escape)) Undo ();
	}


}
