using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour {

	public UnityEvent enter = new UnityEvent();
	public UnityEvent exit  = new UnityEvent();

	void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		enter.Invoke();
	}

	void OnTriggerExit2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		exit.Invoke();
	}
}
