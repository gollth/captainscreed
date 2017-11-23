using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderFilter : Filter {


	public float min = 0;
	public float max = 1;

	protected override float Process (float value, int x, int y) {
		if (value < min) return 0;
		if (value > max) return 0;

		else return 1f;
	}


}
