using UnityEngine;
using System.Collections;

public class KeepRotation : MonoBehaviour {

	public float z = 0;
	
	// Update is called once per frame
	void LateUpdate () {
		this.transform.rotation = Quaternion.Euler (0, 0, z);
	}
}
