using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BorderFilter))]
public class BorderFilterEditor : Editor {

	public override void OnInspectorGUI ()
	{
		var filter = target as BorderFilter;
		EditorGUI.BeginChangeCheck();
		//EditorGUILayout.MinMaxSlider("Border Range", ref filter.min, ref filter.max, 0, 1);
		filter.min = EditorGUILayout.Slider("Min Border", filter.min, 0, filter.max);
		filter.max = EditorGUILayout.Slider("Max Border", filter.max, filter.min, 1f);
		if (!EditorGUI.EndChangeCheck()) return;

		var editors = Resources.FindObjectsOfTypeAll(typeof(GeneratorEditor)) as GeneratorEditor[];
		if (editors.Length == 0) return;
		editors[0].Draw();	
	}



}
