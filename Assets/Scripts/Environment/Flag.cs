using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Flag : MonoBehaviour {

	Wind wind;


	// Use this for initialization
	void OnEnable () {
		wind = GameObject.FindGameObjectWithTag("Wind").GetComponent<Wind>();	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rotation = Quaternion.Euler(0,0, 180 - wind.Direction);
	}
}
