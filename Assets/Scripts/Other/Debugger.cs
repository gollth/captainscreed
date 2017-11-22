using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class Debugger : MonoBehaviour {

	public static void Log (string msg, params object[] values) {
		log += string.Format ("\n"+msg, values);
	}
	private static string log;
	
	Text label;
	
	// Use this for initialization
	void Start () {
		label = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = log;
	}
}
