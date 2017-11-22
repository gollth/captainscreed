using UnityEngine;
using System.Collections;

public class DestroyIfInvisible : MonoBehaviour {

	[Range (0,30), Tooltip ("The delay in seconds to wait, after the gameobject came out of view")]
	public float delay = 0;

	void Start () {
		StartCoroutine (Remove ());
	}

	IEnumerator Remove () {

		while (Util.IsVisibleToCamera (this.transform.position))
			yield return new WaitForEndOfFrame ();

		yield return new WaitForSeconds (delay);

		Destroy (this.gameObject);

	}

}
