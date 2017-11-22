using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter : Filter {

	[Header("Method")]
	[SerializeField] Noise.MethodType method = Noise.MethodType.Value;
	[SerializeField, Range(1, 3)] int dimension = 3;
	[SerializeField, Range(0,10)] int seed = 0;
	[SerializeField] float frequency = 1f;


	[Space(10), Header("Octaves")]
	[SerializeField, Range(1, 10)] int octaves = 1;
	[SerializeField, Range(1,4)] float lacunarity = 2f;
	[SerializeField, Range(0,1)] float persistance = .5f;

	float scale = 1f;

	public override void Initialize () {
		base.Initialize();
		scale = transform.lossyScale.Mean();
	}

	protected override float Process (float value, int x, int y) {
		return Noise.Fractalize(Noise.Methods [method] [dimension - 1], 
			Generator.ImageToWorld(x,y), seed, frequency / scale, octaves, lacunarity, persistance);
	}
}
