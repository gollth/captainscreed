using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ParticleSystem))]
public class Wake : MonoBehaviour {

	[Tooltip ("Particle start Speed (y) over Cruise Speed of Steerman (x)")]
	public AnimationCurve speedBehaviour = AnimationCurve.Linear(0,0,10, 2.5f);

	[Tooltip ("Particle emission rate (y) over Cruise Speed of Steerman (x)")]
	public AnimationCurve emissionBehaviour = AnimationCurve.Linear(0,0,5,50);

	private ParticleSystem particles;
	private Steerman steerman;
	private AudioSource sound;

	void Start() {
		particles = GetComponent<ParticleSystem> ();
		steerman = GetComponentInParent<Steerman> ();
		sound = GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		var parts = particles.main;
		parts.startSpeedMultiplier = speedBehaviour.Evaluate (steerman.Speed);
		
		var em = particles.emission;
		em.rateOverDistance = new ParticleSystem.MinMaxCurve(emissionBehaviour.Evaluate (steerman.Speed));


		if (sound == null) return;
		sound.volume = Mathf.Clamp01(steerman.Speed);
	}
}
