using UnityEditor;

[CustomEditor(typeof(Steerwheel))]
public class SteerwheelEditor : Editor {


	public override void OnInspectorGUI () {

		DrawDefaultInspector ();

		var s = target as Steerwheel;
		if (s == null)	return;
		
		EditorGUILayout.Slider (s.Rudder, -s.rudderLimit, s.rudderLimit);

	}
}
