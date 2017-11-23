using UnityEditor;

[CustomEditor(typeof(Ship))]
public class ShipEditor : Editor {


	private Ship fader { get { return target as Ship; }}

	void OnEnable () { fader.OnRepair += Invalidate; }
	void OnDisable() { fader.OnRepair -= Invalidate; }


	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
	}

	void Invalidate () { EditorUtility.SetDirty (target); }
}
