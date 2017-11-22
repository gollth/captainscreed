using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttenuationFilter))]
public class AttenuationFilterEditor : Editor{


	public override void OnInspectorGUI ()
	{
		var filter = target as AttenuationFilter;
		EditorGUI.BeginChangeCheck();
		filter.center = EditorGUILayout.Vector2Field ("Center", filter.center);
		filter.falloff = (AttenuationFilter.Falloff)EditorGUILayout.EnumPopup ("Falloff Type", filter.falloff);

		if (filter.falloff == AttenuationFilter.Falloff.Constant) {
			filter.param.Set(EditorGUILayout.Slider ("Attenuation", filter.param.x, 0, 2), 0);
		}
		if (filter.falloff == AttenuationFilter.Falloff.Linear || filter.falloff == AttenuationFilter.Falloff.Quadratic) {
			filter.param.Set(EditorGUILayout.Toggle ("Invert", filter.param.x > 0) ? 1f : -1, 0);
		}
		if (filter.falloff == AttenuationFilter.Falloff.Custom) {
			filter.customFalloff = EditorGUILayout.CurveField("Custom Falloff", filter.customFalloff);
		}
		if (!EditorGUI.EndChangeCheck()) return;

		var editors = Resources.FindObjectsOfTypeAll(typeof(GeneratorEditor)) as GeneratorEditor[];
		if (editors.Length == 0) return;
		editors[0].Draw();
	}
	
}

