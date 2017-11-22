using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

[RequireComponent(typeof (Image))]
public class Fader : MonoBehaviour {

	public enum StartFade { OUT, IN, OUT2IN, IN2OUT }

	[SerializeField] bool enableImageOnStart = true;
	[SerializeField] StartFade onStart = StartFade.OUT;

	private Image image;
	public float alpha { 
		get { return image.color.a; } 
		set { image.color = new Color (image.color.r, image.color.g, image.color.b, Mathf.Clamp01 (value)); } 
	}

	void OnEnable () {
		image = GetComponent<Image> ();
		switch (onStart) {
		case StartFade.OUT: 	alpha = 0; break;
		case StartFade.IN:	   	alpha = 1; break;
		case StartFade.OUT2IN:	FadeIn();  break;
		case StartFade.IN2OUT:  FadeOut(); break;
		}

		if (enableImageOnStart) image.enabled = true;

	}
		
	public void FadeIn (Action callback = null) { Fade (1, callback);}
	public void FadeOut (Action callback = null) { Fade (0, callback); }
	public void Fade (float t, Action callback = null) { 
		if (!Application.isPlaying) return;
		Go.to (this, duration: 1.5f, config: new GoTweenConfig ()
			.floatProp ("alpha", endValue: 1 - Mathf.Clamp01 (t))
			.setEaseType (GoEaseType.ExpoOut)
			.setUpdateType(GoUpdateType.TimeScaleIndependentUpdate)
			.onComplete (_ => { if (callback != null) callback (); })
		);
	}
}
