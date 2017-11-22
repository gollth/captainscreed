using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof (RectTransform))]
public class ScreenSizer : MonoBehaviour {

	public Vector2 size = -Vector2.one;

	CanvasScaler scaler;
	Canvas canvas;

	void OnEnable () {
		// Ensure to call at least once on startup
		canvas = GetComponentInParent<Canvas> ();
		scaler = GetComponentInParent<CanvasScaler> ();
		if (canvas == null || scaler == null) {
			var ui = GameObject.FindGameObjectWithTag ("UI");
			canvas = ui.GetComponent<Canvas> ();
			scaler = ui.GetComponent<CanvasScaler> ();
		}
		
		ApplyOffset();

		if (scaler.matchWidthOrHeight == 0 || scaler.matchWidthOrHeight == 1) return;
		Debug.LogWarningFormat ("{0}: CanvasScaler component not having 0 or 1 value for Match Width or Height. ScreenSizer might produce invalid results!", name);
	}

	// Update is called once per frame
	void Update () {
		//if (Application.isPlaying) return;	// Dont continue when playing;
		//if (!transform.hasChanged) return;
		ApplyOffset();
		//transform.hasChanged = false;
	}

	void ApplyOffset() {
		var rt = this.transform as RectTransform;

		if (canvas.renderMode == RenderMode.WorldSpace) {
			var s = Vector2.one * Camera.main.orthographicSize;
			if (size.x >= 0) s.x *= size.x;
			if (size.y >= 0) s.y *= size.y;
			rt.sizeDelta = s;

		} else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
			rt.sizeDelta = Util.ScreenSizePx.ScaleEach(size);
			//var s = Util.ScreenSizePx.ScaleEach(size);
			//rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, s.x);
			//rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   s.y);
		}
	}
}
