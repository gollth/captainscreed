using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor {

	static List<GeneratorEditor> instances = new List<GeneratorEditor>();

	IEnumerator worker;
	Generator generator;
	bool oldlive;

	void OnEnable() {
		instances.Add(this);
		generator = target as Generator;
		oldlive = generator.Live;
		Undo.undoRedoPerformed += Draw;
		EditorApplication.update += Refresh;
		generator.OnTransformChanged += Draw;
	}

	void OnDisable() {
		instances.Remove(this);
		Undo.undoRedoPerformed -= Draw;
		EditorApplication.update -= Refresh;
		generator.OnTransformChanged -= Draw;
	}

	public void Draw () {
		generator.Generate();
	}
	void Refresh() {
		if (Application.isPlaying) return;
		if (!generator.Working) return;
		Repaint();
	}

	[DidReloadScripts]
	static void OnRecompiled() {
		foreach (var instance in instances) instance.Draw();
	}


	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();
		DrawDefaultInspector ();

		generator.display = EditorGUILayout.ObjectField ("Pipeline", generator.display, typeof(GameObject), true) as GameObject;
		if (generator.display != null && !generator.display.transform.IsChildOf (generator.transform)){
				EditorGUILayout.HelpBox ("Pipeline Leaf must be a child of this GameObject!", MessageType.Error);
		}

		generator.Live = GUILayout.Toggle (generator.Live, "Live", "Button");

		var r = EditorGUILayout.BeginVertical();
		var p = generator.Progress;
		var s = "";
		if (generator.CurrentFilter != null) s = generator.CurrentFilter.GetType().Name;
		if (p >= .999f) s = "OK";
     	EditorGUI.ProgressBar(r, p, s);
     	GUILayout.Space(16);

     	EditorGUILayout.EndVertical();
		if (generator.Live && !oldlive) {
			Draw();
			oldlive = generator.Live; 
		}
		else if (oldlive && !generator.Live) 
			generator.Stop();
		else if (EditorGUI.EndChangeCheck ()) 
			Draw();
		

		oldlive = generator.Live; 
	}
}
