using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Collider2D)), RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour {

	[Range(0,5)] float triggerActivationDelay = 1f;

	AudioSource source;
	Collider2D trigger;
	SpriteRenderer image;

	IEnumerator Start() {
		source = GetComponent<AudioSource>();
		trigger = GetComponent<Collider2D>();
		image  = GetComponent<SpriteRenderer>();

		trigger.enabled = false;
		yield return new WaitForSeconds(triggerActivationDelay);
		trigger.enabled = true;
	}

	void OnTriggerEnter2D (Collider2D other) {

		if (!other.CompareTag ("Player")) return;


		GameObject.FindGameObjectWithTag ("Goldpot").GetComponent<Goldpot> ()
			.StoreCoinInHold ();


		source.enabled = true;
		trigger.enabled = false;
		image.enabled = false;


		Destroy (this.gameObject, source.clip.length);
	}
}
