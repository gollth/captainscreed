using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UpgradeLine : MonoBehaviour {

	[SerializeField] string targetNameInShip;
	LineRenderer line;
	Transform target;


	void OnEnable() {
		line = GetComponent<LineRenderer>();
		line.enabled = false;
		ResourceLoader.WaitFor ("Player", player => {
			target = player.transform.Find(targetNameInShip);
			print("Found " + target.name);
			line.enabled = true;
		});
	}

	// Update is called once per frame
	void Update () {
		if (target == null) return;
		line.SetPosition(2, transform.InverseTransformPoint(target.position));
	}
}
