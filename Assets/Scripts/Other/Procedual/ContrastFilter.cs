using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrastFilter : Filter {

	[SerializeField] AnimationCurve contrast = AnimationCurve.Linear(0,0,1,1);

	protected override float Process (float value, int x, int y) {
		return contrast.Evaluate(value);
	}


}
