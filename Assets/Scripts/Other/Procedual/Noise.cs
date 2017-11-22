using UnityEngine;
using System;
using System.Collections.Generic;

public delegate float Method (Vector3 point, float frequency=1f, int seed=0);

public static class Noise {

	#region Helpers
	static int[] hash = {
		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,
		// repeated for preventing index out of bounds errors
		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
	};

	const int mask = 255;
	// 6t^5 - 15t^4 + 10t^3
	static float cubic (float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

	public static float Fractalize (Method method, Vector3 point, int seed = 0, float frequency = 1f, int octaves = 1, float lacunarity=2f, float persistance=.5f) {
		var sum = (2*method (point, frequency, seed)-1);
		var amplitude  = 1f;
		var range = 1f;
		for (int o = 1; o < octaves; o++) {
			amplitude *= persistance;
			frequency *= lacunarity;
			range     += amplitude;
			sum       += (2*method (point, frequency)-1) * amplitude;
		}
		return .5f * (sum / range + 1);
	}
	#endregion

	#region API
	public enum MethodType { Value, Perlin, Simplex };

	public static readonly Dictionary<MethodType, Method[]> Methods = new Dictionary<MethodType, Method[]> { 
		{ MethodType.Value, new Method[]{ Value1D, Value2D, Value3D }},
		{ MethodType.Perlin, new Method[] { Perlin1D, Perlin2D, Perlin3D }},
		{ MethodType.Simplex, new Method[] {Simplex1D, Simplex2D, Simplex3D }}
	};
	#endregion

	#region Value Noise
	public static float Value1D (Vector3 point, float frequency = 1f, int seed=0) {
		point *= frequency;
		int i0 = Mathf.FloorToInt (point.x);
		var t = point.x - i0;
		i0 += seed * 1000;
		i0 &= mask;
		int i1 = i0 + 1;
		return Mathf.Lerp (hash [i0], hash [i1], cubic(t)) / mask;
	}

	public static float Value2D (Vector3 point, float frequency = 1f, int seed = 0) {
		point *= frequency;
		var ix0 = Mathf.FloorToInt (point.x);
		var iy0 = Mathf.FloorToInt (point.y);

		var tx = cubic(point.x - ix0);
		var ty = cubic(point.y - iy0);

		ix0 += seed * 1000;
		iy0 += seed * 1000;

		ix0 &= mask;
		iy0 &= mask;
		var ix1 = ix0 + 1;
		var iy1 = iy0 + 1;

		var h0 = hash [ix0];
		var h1 = hash [ix1];

		var h00 = hash [h0 + iy0];
		var h01 = hash [h0 + iy1];
		var h10 = hash [h1 + iy0];
		var h11 = hash [h1 + iy1];

		return Mathf.Lerp (
			Mathf.Lerp(h00, h10, tx),
			Mathf.Lerp(h01, h11, tx),
			ty
		) / mask;
	}

	public static float Value3D (Vector3 point, float frequency = 1f, int seed = 0) {
		point *= frequency;
		var ix0 = Mathf.FloorToInt (point.x);
		var iy0 = Mathf.FloorToInt (point.y);
		var iz0 = Mathf.FloorToInt (point.z);

		var tx = cubic(point.x - ix0);
		var ty = cubic(point.y - iy0);
		var tz = cubic (point.z - iz0);

		ix0 += seed * 1000;
		iy0 += seed * 1000;
		iz0 += seed * 1000;

		ix0 &= mask;
		iy0 &= mask;
		iz0 &= mask;

		var ix1 = ix0 + 1;
		var iy1 = iy0 + 1;
		var iz1 = iz0 + 1;

		var hx0 = hash [ix0];
		var hx1 = hash [ix1];

		var h00 = hash [hx0 + iy0];
		var h01 = hash [hx0 + iy1];
		var h10 = hash [hx1 + iy0];
		var h11 = hash [hx1 + iy1];

		var h000 = hash [h00 + iz0];
		var h100 = hash [h10 + iz0];
		var h010 = hash [h01 + iz0];
		var h110 = hash [h11 + iz0];
		var h001 = hash [h00 + iz1];
		var h101 = hash [h10 + iz1];
		var h011 = hash [h01 + iz1];
		var h111 = hash [h11 + iz1];

		return Mathf.Lerp(
			Mathf.Lerp (
				Mathf.Lerp(h000, h100, tx),
				Mathf.Lerp(h010, h110, tx),
				ty
			),
			Mathf.Lerp (
				Mathf.Lerp(h001, h101, tx),
				Mathf.Lerp(h011, h111, tx),
				ty
			),
			tz
		) / mask;
	}

	#endregion

	#region Perlin Noise
	static readonly float[] gradients1D = { -1f, +1f };
	const int maskg1D = 1;

	public static float Perlin1D (Vector3 point, float frequency = 1f, int seed=0) {
		point *= frequency;
		int i0 = Mathf.FloorToInt (point.x);
		var t0 = point.x - i0;
		var t1 = t0 - 1;
		i0 += seed * 1000;
		i0 &= mask;
		int i1 = i0 + 1;

		var g0 = gradients1D [hash [i0] & maskg1D];
		var g1 = gradients1D [hash [i1] & maskg1D];

		return Mathf.Lerp (t0 * g0, t1 * g1, cubic (t0)) + .5f;
	}

	static readonly Vector2[] gradients2D = { 
		Vector2.up,
		(Vector2.up + Vector2.right).normalized,
		Vector2.right,
		(Vector2.right + Vector2.down).normalized,
		Vector2.down,
		(Vector2.down + Vector2.left).normalized,
		Vector2.left,
		(Vector2.left + Vector2.up).normalized
	};
	const int maskg2D = 7;
	const float sqrt2 = 1.4142136f;
	const float sqrt3 = 1.73205080757f;
	static float dot(Vector2 g, float x, float y) { return g.x * x + g.y * y; }

	public static float Perlin2D (Vector3 point, float frequency = 1f, int seed=0) {
		point *= frequency;
		var ix0 = Mathf.FloorToInt (point.x);
		var iy0 = Mathf.FloorToInt (point.y);

		var tx0 = point.x - ix0;
		var ty0 = point.y - iy0;

		var tx1 = tx0 - 1;
		var ty1 = ty0 - 1;

		ix0 += seed * 1000;
		iy0 += seed * 1000;
		ix0 &= mask;
		iy0 &= mask;

		var ix1 = ix0 + 1;
		var iy1 = iy0 + 1;

		var h0 = hash [ix0];
		var h1 = hash [ix1];
		var g00 = gradients2D [hash [h0 + iy0] & maskg2D];
		var g10 = gradients2D [hash [h1 + iy0] & maskg2D];
		var g01 = gradients2D [hash [h0 + iy1] & maskg2D];
		var g11 = gradients2D [hash [h1 + iy1] & maskg2D];

		var v00 = dot (g00, tx0, ty0);
		var v10 = dot (g10, tx1, ty0);
		var v01 = dot (g01, tx0, ty1);
		var v11 = dot (g11, tx1, ty1);

		var tx = cubic(tx0);
		var ty = cubic(ty0);

		return Mathf.Lerp (
			Mathf.Lerp(v00, v10, tx),
			Mathf.Lerp(v01, v11, tx),
			ty
		) * sqrt2 / 2 + .5f;
	}

	static Vector3[] gradients3D = {
		new Vector3( 1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3( 1f,-1f, 0f),
		new Vector3(-1f,-1f, 0f),
		new Vector3( 1f, 0f, 1f),
		new Vector3(-1f, 0f, 1f),
		new Vector3( 1f, 0f,-1f),
		new Vector3(-1f, 0f,-1f),
		new Vector3( 0f, 1f, 1f),
		new Vector3( 0f,-1f, 1f),
		new Vector3( 0f, 1f,-1f),
		new Vector3( 0f,-1f,-1f),
		
		new Vector3( 1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3( 0f,-1f, 1f),
		new Vector3( 0f,-1f,-1f)
	};
	const int maskg3D = 15;
	static float dot(Vector3 g, float x, float y, float z) { return g.x * x + g.y * y + g.z * z; }

	public static float Perlin3D (Vector3 point, float frequency = 1f, int seed=0) {
		point *= frequency;
		var ix0 = Mathf.FloorToInt (point.x);
		var iy0 = Mathf.FloorToInt (point.y);
		var iz0 = Mathf.FloorToInt (point.z);

		var tx0 = point.x - ix0;
		var ty0 = point.y - iy0;
		var tz0 = point.z - iz0;

		var tx1 = tx0 - 1;
		var ty1 = ty0 - 1;
		var tz1 = tz0 - 1;
		ix0 += seed * 1000;
		iy0 += seed * 1000;
		iz0 += seed * 1000;

		ix0 &= mask;
		iy0 &= mask;
		iz0 &= mask;

		var ix1 = ix0 + 1;
		var iy1 = iy0 + 1;
		var iz1 = iz0 + 1;

		var h0 = hash [ix0];
		var h1 = hash [ix1];
		var h00 = hash [h0 + iy0];
		var h01 = hash [h0 + iy1];
		var h10 = hash [h1 + iy0];
		var h11 = hash [h1 + iy1];

		var g000 = gradients3D [hash [h00 + iz0] & maskg3D];
		var g001 = gradients3D [hash [h00 + iz1] & maskg3D];
		var g010 = gradients3D [hash [h01 + iz0] & maskg3D];
		var g011 = gradients3D [hash [h01 + iz1] & maskg3D];
		var g100 = gradients3D [hash [h10 + iz0] & maskg3D];
		var g101 = gradients3D [hash [h10 + iz1] & maskg3D];
		var g110 = gradients3D [hash [h11 + iz0] & maskg3D];
		var g111 = gradients3D [hash [h11 + iz1] & maskg3D];

		var v000 = dot (g000, tx0, ty0, tz0);
		var v001 = dot (g001, tx0, ty0, tz1);
		var v010 = dot (g010, tx0, ty1, tz0);
		var v011 = dot (g011, tx0, ty1, tz1);
		var v100 = dot (g100, tx1, ty0, tz0);
		var v101 = dot (g101, tx1, ty0, tz1);
		var v110 = dot (g110, tx1, ty1, tz0);
		var v111 = dot (g111, tx1, ty1, tz1);

		var tx = cubic (tx0);
		var ty = cubic (ty0);
		var tz = cubic (tz0);

		return Mathf.Lerp( 
			Mathf.Lerp (
				Mathf.Lerp(v000, v100, tx),
				Mathf.Lerp(v010, v110, tx),
				ty
			),
			Mathf.Lerp (
				Mathf.Lerp(v001, v101, tx),
				Mathf.Lerp(v011, v111, tx),
				ty
			), 
			tz
		) * sqrt2 / 2 +.5f;
	}

	#endregion

	#region Simplex
	public static float Simplex1D (Vector3 point, float frequency = 1f, int seed = 0)
	{
		point *= frequency;
		var ix = Mathf.FloorToInt(point.x);
			
		Func<int, float> falloff = i => {
			var x = point.x - i;
			var f = 1f - x*x;
			var g = gradients1D[hash[(i + seed*1000) & mask] & maskg1D];
			return f * f * f * g * x;
		};

		return (falloff(ix) + falloff(ix+1)) * 32f / 27f +.5f;
	}

	readonly static float square2tria = (3f - sqrt3) / 6f;
	readonly static float tria2square = (sqrt3 - 1f) / 2f;
	readonly static float simplexScale2D = 2916f * sqrt2 / 125f;
	public static float Simplex2D (Vector3 point, float frequency = 1f, int seed = 0)
	{
		point *= frequency;
		var skew = (point.x + point.y) * tria2square;
		var s = point + new Vector3(1,1,0) * skew;
		var ix = Mathf.FloorToInt(s.x);
		var iy = Mathf.FloorToInt(s.y);

		Func<int, int, float> falloff = (i,j) => {
			var unskew = (i+j) * square2tria;
			var x = point.x - i + unskew;
			var y = point.y - j + unskew;
			var f = .5f - x*x - y*y;
			i += seed * 1000;
			j += seed * 1000;
			i &= mask;
			j &= mask;
			var g = gradients2D[hash[hash[i] + j] & maskg2D];
			return dot(g, x, y) * f * f * f;
		};
		var sample = falloff(ix, iy) + falloff(ix+1, iy+1);
		if (s.x - ix >= s.y - iy) sample += falloff(ix+1, iy);
		else                      sample += falloff(ix, iy+1); 
		return sample * simplexScale2D / 2 +.5f;
	}



	static readonly Vector3[] simplexGradients3D = { 
		new Vector3( 1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3( 1f,-1f, 0f).normalized,
		new Vector3(-1f,-1f, 0f).normalized,
		new Vector3( 1f, 0f, 1f).normalized,
		new Vector3(-1f, 0f, 1f).normalized,
		new Vector3( 1f, 0f,-1f).normalized,
		new Vector3(-1f, 0f,-1f).normalized,
		new Vector3( 0f, 1f, 1f).normalized,
		new Vector3( 0f,-1f, 1f).normalized,
		new Vector3( 0f, 1f,-1f).normalized,
		new Vector3( 0f,-1f,-1f).normalized,
		
		new Vector3( 1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3( 1f,-1f, 0f).normalized,
		new Vector3(-1f,-1f, 0f).normalized,
		new Vector3( 1f, 0f, 1f).normalized,
		new Vector3(-1f, 0f, 1f).normalized,
		new Vector3( 1f, 0f,-1f).normalized,
		new Vector3(-1f, 0f,-1f).normalized,
		new Vector3( 0f, 1f, 1f).normalized,
		new Vector3( 0f,-1f, 1f).normalized,
		new Vector3( 0f, 1f,-1f).normalized,
		new Vector3( 0f,-1f,-1f).normalized,
		
		new Vector3( 1f, 1f, 1f).normalized,
		new Vector3(-1f, 1f, 1f).normalized,
		new Vector3( 1f,-1f, 1f).normalized,
		new Vector3(-1f,-1f, 1f).normalized,
		new Vector3( 1f, 1f,-1f).normalized,
		new Vector3(-1f, 1f,-1f).normalized,
		new Vector3( 1f,-1f,-1f).normalized,
		new Vector3(-1f,-1f,-1f).normalized
	};
	const int masksimplexg3D = 31;

	readonly static float simplexScale3D = 8192f * sqrt3 / 375f;
	public static float Simplex3D (Vector3 point, float frequency = 1f, int seed =0 )
	{
		point *= frequency;
		float skew = (point.x + point.y + point.z) * (1f / 3f);
		float sx = point.x + skew;
		float sy = point.y + skew;
		float sz = point.z + skew;
		int ix = Mathf.FloorToInt(sx);
		int iy = Mathf.FloorToInt(sy);
		int iz = Mathf.FloorToInt(sz);

		Func<int, int, int, float> falloff = (i, j, k) => {
			float unskew = (i + j + k) * (1f / 6f);
			float x = point.x - i + unskew;
			float y = point.y - j + unskew;
			float z = point.z - k + unskew;
			float f = 0.5f - x * x - y * y - z * z;
			if (f < 0) return 0f;
			i += seed * 1000;
			j += seed * 1000;
			k += seed * 1000;
			i &= mask;
			j &= mask;
			k &= mask;
			var g = simplexGradients3D[hash[hash[hash[i] + j] + k] & masksimplexg3D];
			return f * f * f * dot(g, x,y,z);
		};
		var sample = falloff (ix, iy, iz) + falloff (ix+1, iy+1, iz+1);
		float dx = sx - ix;
		float dy = sy - iy;
		float dz = sz - iz;
		if (dx >= dy) {
			if (dx >= dz) {
				sample += falloff(ix + 1, iy, iz);
				if (dy >= dz) {
					sample += falloff(ix + 1, iy + 1, iz);
				}
				else {
					sample += falloff(ix + 1, iy, iz + 1);
				}
			}
			else {
				sample += falloff(ix, iy, iz + 1);
				sample += falloff(ix + 1, iy, iz + 1);
			}
		}
		else {
			if (dy >= dz) {
				sample += falloff(ix, iy + 1, iz);
				if (dx >= dz) {
					sample += falloff(ix + 1, iy + 1, iz);
				}
				else {
					sample += falloff(ix, iy + 1, iz + 1);
				}
			}
			else {
				sample += falloff(ix, iy, iz + 1);
				sample += falloff(ix, iy + 1, iz + 1);
			}
		}
		return sample * simplexScale3D / 2f + .5f;
	}
	#endregion

}