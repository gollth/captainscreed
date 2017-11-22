using UnityEngine;
using UnityEngine.UI;

using System.Linq;

public class LevelMenu : MonoBehaviour {

	private Button[] buttons;

	// Use this for initialization
	void Awake () {
		buttons = GetComponentsInChildren<Button> ();
		if (buttons == null) return;

		// Title Scene
		var level = Level.Number;
		foreach (var button in buttons) {
			var buttonLevel = Util.GetLevelFromScene (button.name);
			if (buttonLevel == -1) continue;
			button.gameObject.SetActive (level >= buttonLevel);
			var color = Color.white;
			if (buttonLevel < level) color =  new Color(0.58f, 0.827f, 0.298f);	 // mint green
			button.GetComponentsInChildren <Image>().First(item => item.GetComponent<Route>() == null).color = color;
			button.GetComponentInChildren <Blinker>().enabled = level == buttonLevel;
		}
	}

}
