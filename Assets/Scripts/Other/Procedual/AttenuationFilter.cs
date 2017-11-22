using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttenuationFilter : Filter {

	public enum Falloff { Constant, Linear, Quadratic, Custom } 
	public Falloff falloff;
	public Vector2 param = Vector2.one;
	public AnimationCurve customFalloff = AnimationCurve.Linear(0,0,1,1);
	public Vector2 center = .5f * Vector2.one;

	Vector2 origin;
	float resolution;

	float maxdistance2;
	float maxdistance;

	public override void Initialize () {
		base.Initialize();
		resolution = Generator.Resolution;
		origin = center * resolution;
		maxdistance = Mathf.Max(
			origin.magnitude,
			(new Vector2(resolution, 0) - origin).magnitude,
			(new Vector2(0, resolution) - origin).magnitude,
			(Vector2.one * resolution   - origin).magnitude
		);
		maxdistance2 = maxdistance * maxdistance;
	}

	protected override float Process (float value, int x, int y)
	{
		Vector2 delta = new Vector2(x,y) - origin;
		float attenuation = 0f;
		switch (falloff) {
		case Falloff.Constant:  return param.x * value; 
		case Falloff.Linear:    attenuation = delta.magnitude / maxdistance; break;
		case Falloff.Quadratic: attenuation = delta.sqrMagnitude / maxdistance2; break;
		case Falloff.Custom :   attenuation = customFalloff.Evaluate(delta.magnitude / maxdistance); break;
		}
		if (param.x < 0) return value * (1f-attenuation);
		else return value * attenuation;
	}

}
