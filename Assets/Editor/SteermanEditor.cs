using UnityEditor;

[CustomEditor(typeof(Steerman))]
public class SteermanEditor : Editor {

	public override void OnInspectorGUI () {
		var s = target as Steerman;
		DrawDefaultInspector ();
		if (s == null)
			return;
		s.Rudder = EditorGUILayout.Slider ("Rudder", s.Rudder, -90, 90);
		EditorGUILayout.LabelField ("Speed", string.Format ("{0:0.0} kn", s.Speed));
		EditorGUILayout.LabelField ("Boost", string.Format ("{0:0} %", 100 * s.maxSpeed * (1 + s.sailorBoost)));
		EditorGUILayout.LabelField ("Luv Angle", string.Format ("{0:0} deg", Util.Mod180 (s.GetLuvAngle())));
	}
}
