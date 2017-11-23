using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMover : MonoBehaviour {

	[SerializeField] Vector2 movement = Vector2.zero;
	[SerializeField] float time = 1;
	[SerializeField] bool onStart = true;
	[SerializeField] GoEaseType easing = GoEaseType.BounceOut;


	public float X { get { return movement.x; } set { movement = new Vector2 (value, movement.y); } }
	public float Y { get { return movement.y; } set { movement = new Vector2 (movement.x, value); } }

	// Use this for initialization
	void Start () {
		if (onStart) Execute ();
	}

	public void Execute() { Execute (null); }
	public void Execute(Action callback) {
		if (!this.enabled) return;

		var s = (Vector2)transform.lossyScale;
		Go.to (transform, duration: time, config: new GoTweenConfig ()
			.localPosition (movement.ScaleEach(Util.ScreenSize).ScaleEach(s.Inverse()), isRelative: true)
			.setEaseType (easing)
			.setUpdateType(GoUpdateType.TimeScaleIndependentUpdate)
			.setTimeScale(1)
			.onComplete (_ => { if (callback != null) callback (); })
		);
	}

	public void Reverse() { Reverse (null); }
	public void Reverse(Action callback) {
		if (!this.enabled) return;
		var s = (Vector2)transform.lossyScale;
		Go.to (transform, duration: time, config: new GoTweenConfig ()
			.localPosition (-movement.ScaleEach(Util.ScreenSize).ScaleEach(s.Inverse()), isRelative: true)
			.setEaseType (easing)
			.setUpdateType(GoUpdateType.TimeScaleIndependentUpdate)
			.setTimeScale(1)
			.onComplete (_ => { if (callback != null) callback (); })
		);
	}

}
