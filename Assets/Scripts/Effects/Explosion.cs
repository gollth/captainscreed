using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public static GameObject Detonate (Vector3 position, float size = 1f, bool mute = false) {
		var explosion = (GameObject) Instantiate (
			Resources.Load ("explosion"), position, Quaternion.identity);
		explosion.transform.localScale *= size;
		explosion.GetComponent<AudioSource> ().volume *= mute ? 0 : size;
		explosion.transform.parent = GameObject.FindGameObjectWithTag("Particles").transform;
		return explosion;
	}
		

	public void OnAnimationFinsihed () {
		Destroy (this.gameObject);
	}


}
