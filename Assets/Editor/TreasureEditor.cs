using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Treasure))]
public class TreasureEditor : Editor {

	public override void OnInspectorGUI () {

		var t = target as Treasure;
		DrawDefaultInspector ();

		if (t.quest.Equals (Treasure.Quest.Ram) || t.quest.Equals (Treasure.Quest.Reach)) {

			var col = t.GetComponent<Collider2D> ();
			if (col == null) {
				EditorGUILayout.HelpBox ("This GameObject has no collider attached", MessageType.Warning);
			} else if (t.quest.Equals (Treasure.Quest.Reach) && !col.isTrigger) {
				EditorGUILayout.HelpBox ("The " + col.GetType().ToString() + " is not a trigger", MessageType.Info); 
			}
		}

		

	}

}
