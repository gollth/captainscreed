using UnityEngine;
using UnityEngine.UI;

using System.Collections;


[ExecuteInEditMode]
public class Wind : MonoBehaviour {

	public float Direction { get { return transform.localEulerAngles.z; }}
	public float Strength = 1f; 
	public Image windUI;
	[Range(0,90)]
	public float UIRandomness = 5;

	private Color color = Color.yellow;
	private float uiWindDirection;

	void Start () {
		Invoke ("OnWindSquall",0);
	}

	public void UpdateUI () {
		uiWindDirection = -Direction + Random.Range (-UIRandomness, UIRandomness);	// wind changes +/- 5 degrees

	}

	void OnWindSquall () {
		UpdateUI ();
		Invoke ("OnWindSquall", Random.Range (0.5f, 3f));	// Wind changes every .5 ... 3 seconds
	}

	// Update is called once per frame
	void Update () {
		Util.DrawArrow (transform.position, Direction, Strength, color);
		if (windUI != null)
			windUI.rectTransform.localRotation = Quaternion.Slerp (
				windUI.rectTransform.localRotation,
				Quaternion.Euler (uiWindDirection * Vector3.forward),
				2f * Time.deltaTime
			);
	}
}
