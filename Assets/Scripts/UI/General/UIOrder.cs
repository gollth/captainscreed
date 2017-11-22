using UnityEngine;


public class UIOrder : MonoBehaviour {

	[Range (0,20)]
	public int offset = 0;
	public bool fromFirstNotLast = true;

	// Update is called once per frame
	void Start () {
		var index = Mathf.Clamp (offset, 0, transform.parent.childCount-1);
		if (!fromFirstNotLast) index = transform.parent.childCount-1 - index;
		
		transform.SetSiblingIndex (index);
	}
}
