using UnityEngine;
using System.Collections.Generic;

public class Crownest : MonoBehaviour {

	[Range(0, 10), Tooltip ("This factor gets multiplied with the \"size\" property of the main camera. The higher this value, the more zoom out effect")]
	public float zoomOut = 2;
	[Tooltip ("The higher this value, the faster you will reach the crownest (zoom)")]
	public float climbSpeed = 5;

	public bool Zoomed { 
		get  { return zoomed; } 
		set { 
			zoomed = value; 
			targetSize = zoomed ? originalSize * zoomOut : originalSize; 
			if (!zoomed) { // anymore
				Fabric.Answers.Answers.LogCustom ("Crownest", new Dictionary<string, object> () {
					{ "Time", timeInCrownest },
					{ "Level", manager.Current.name }
				});
			}
		} 
	}

	private SceneChanger manager;
	private bool zoomed;
	private float targetSize;
	private float originalSize;
	private float timeInCrownest;

	// Use this for initialization
	void Start () {
		originalSize = Camera.main.orthographicSize;
		targetSize = originalSize;
		manager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<SceneChanger> ();
		timeInCrownest = 0;
		
	}

	// Update is called once per frame
	void Update () {
		Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, targetSize, climbSpeed * Time.deltaTime);
		if (Zoomed) timeInCrownest += Time.deltaTime;
	}
}
