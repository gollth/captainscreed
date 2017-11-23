using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvertFilter : Filter {


	protected override float Process (float value, int x, int y) {
		return 1f - value;
	}

}
