using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Fader))]
public class FaderEditor : Editor {


	private Fader fader { get { return target as Fader; }}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		if (GUILayout.Button ("Fade Out")) fader.FadeOut ();
		if (GUILayout.Button ("Fade In"))  fader.FadeIn ();
	}

}

