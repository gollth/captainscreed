using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Fire : MonoBehaviour {

	float healthThreshold = .33f;

	Ship ship;
	ParticleSystem.EmissionModule system;

	void OnEnable() {
		ship = GetComponentInParent<Ship>();
		system = GetComponent<ParticleSystem>().emission;
		system.enabled = false;
		ship.OnDamage += TakeDamage;
		ship.OnRepair += OnRepair;
	}

	void OnDisable () {
		system.enabled = false;
		ship.OnDamage -= TakeDamage;
		ship.OnRepair -= OnRepair;
	}

	void TakeDamage(GameObject enemy, int damage) {
		if (ship.RelativeHealth > healthThreshold) return;
		system.enabled = true;
	}

	void OnRepair() {
		if (ship.RelativeHealth <= healthThreshold) return;
		system.enabled = false;
	}

}
