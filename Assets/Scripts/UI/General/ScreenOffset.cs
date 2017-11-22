using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode, RequireComponent(typeof (RectTransform))]
public class ScreenOffset : MonoBehaviour {

	public Vector2 offset;

	bool ok;

	void OnEnable () { ok = false; }

	void LateUpdate () {
		if (!Application.isPlaying) { ApplyOffset (); return; }
		if (ok)	return;

		ApplyOffset ();
		ok = true;
	}

	void ApplyOffset() {
		var rt = this.transform as RectTransform;
		if (offset.Is (float.IsInfinity)) return;
		var pos = rt.localPosition;
		var z = rt.localPosition.z;
		pos = (Vector3)Util.ScreenSize.ScaleEach(offset);
		pos.Scale(rt.lossyScale.Inverse());
		pos.z = z;
		rt.localPosition = pos;
	}
}
