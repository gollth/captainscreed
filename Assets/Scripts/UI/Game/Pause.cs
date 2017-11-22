using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Pause : MonoBehaviour {

	void OnEnable() {
		var button = GetComponent<Button>();
		ResourceLoader.WaitFor <Menu>("Menu",
			callback: menu => button.onClick.AddListener (menu.Toggle)
		);
	}
}
