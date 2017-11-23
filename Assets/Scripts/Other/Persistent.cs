using UnityEngine;
using System.Collections;

public class Persistent : MonoBehaviour {

	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
}
