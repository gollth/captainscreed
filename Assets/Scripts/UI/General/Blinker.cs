using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Graphic))]
public class Blinker : MonoBehaviour {

	public Color highlight = Color.red;
	[Range (0,15)]
	public float frequency = 2;
	
	
	Graphic graphic;
	Color original;
	
	// Use this for initialization
	void Start () {
		graphic = GetComponent<Graphic>();
		original = graphic.color;
	}
	
	// Update is called once per frame
	void Update () {
		graphic.color = Color.Lerp (original, highlight, .5f * Mathf.Sin (2* Mathf.PI * frequency * Time.time) + .5f);
				
	}
}
