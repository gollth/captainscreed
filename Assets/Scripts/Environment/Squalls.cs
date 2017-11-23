using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Wind))]
public class Squalls : MonoBehaviour {

	#region Tweakables
	[Range (0,5)]
	public float frequency = 1;
	[Range (0,1)]
	public float frequencyRandomness = 0.2f;

	[Range (0,180), Tooltip ("The maximal deviation of the wind direction in °")]
	public float maxDeviation = 45;

	#endregion
	private Wind wind;
	private Quaternion original;
	private float timer;
	private float originalFreq;

	// Use this for initialization
	void Start () {
		wind = GetComponent<Wind> ();
		original = transform.rotation;
		originalFreq = frequency;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > 1f / frequency) {
			timer = 0;
		
			// Create a new wind squall change
			transform.rotation = original * Quaternion.Euler(0,0,Random.Range (-maxDeviation, maxDeviation));
			wind.UpdateUI ();

			frequency = originalFreq * frequencyRandomness * Random.Range (-originalFreq, originalFreq);
		}
	}

}
