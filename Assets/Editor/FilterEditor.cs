using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Filter), true)]
public class FilterEditor : Editor {

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (!EditorGUI.EndChangeCheck()) return;

		var editors = Resources.FindObjectsOfTypeAll(typeof(GeneratorEditor)) as GeneratorEditor[];
		if (editors.Length == 0) return;
		editors[0].Draw();
	}
}

