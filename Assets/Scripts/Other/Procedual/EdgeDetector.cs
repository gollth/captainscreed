using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetector : Filter {

	[SerializeField] float threshold = -1f;

	float[,] mask = {{-1, 0, 1},
	                 {-2, 0, 2},
	                 {-1, 0, 1}};

	protected override float Process (float _, int x, int y) {
		// if mask does not fit completely on edge, return black
		if (x < 1 || y < 1 || x >= Generator.Resolution-1 || y >= Generator.Resolution-1) return 0;

		float v = 0, h = 0;
		for (int b = 0; b < 3; b++) {
			for (int a = 0; a < 3; a++) {
				var value = Generator.Samples[x-a+1,y-b+1];
				h += value * mask [a, b];
				v += value * mask [b, a];
			}
		}

		var edge = Mathf.Sqrt (v * v + h * h);
		if (threshold < 0) return edge;	    // no thresholding
		if (edge < threshold) return 0f;	// no edge

		else return 1f;
	}

}
