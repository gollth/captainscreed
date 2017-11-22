using UnityEditor;

[CustomEditor (typeof (Cannons))] 
public class CannonsEditor : Editor {

	private Cannons Cannons { get { return target as Cannons; } }

	void OnEnable() {
		Cannons.OnReload += Invalidate;
	}
	void OnDisable () {
		Cannons.OnReload -= Invalidate;
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		EditorGUILayout.Separator ();

		Cannons.Fire = EditorGUILayout.Toggle ("Fire", Cannons.Fire); 
		EditorGUILayout.LabelField ("Ready", Cannons.Ready + " of " + Cannons.amount); 


	}

	void Invalidate () {
		EditorUtility.SetDirty (target);
	}
}
