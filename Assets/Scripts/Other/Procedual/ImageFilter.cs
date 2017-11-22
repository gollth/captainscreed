using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFilter : Filter {

	[SerializeField] Texture2D tex;


	protected override float Process (float value, int x, int y) {
		if (x < 0 || x > tex.width || y < 0 || y > tex.height) return 0;
		return tex.GetPixel(x,y).grayscale;
	}

}
