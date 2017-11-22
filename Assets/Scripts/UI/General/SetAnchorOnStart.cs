using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class SetAnchorOnStart : MonoBehaviour {

	public enum Preset {
		KeepAsItIs,
		Bottom
	}

	public Preset preset;
	
	// Use this for initialization
	void Start () {
		
		var rect = GetComponent<RectTransform> ();
		switch (preset) {
		case Preset.Bottom:
			
			rect.anchorMin = new Vector2 (.5f, 0);
			rect.anchorMax = new Vector2 (.5f, 0);
			rect.SetSize((rect.parent.parent as RectTransform).GetSize());
			rect.anchoredPosition = new Vector2 (0, rect.sizeDelta.y / 2);
			break;
		}
	}
}
