using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DecimationFilter : Filter {


	[SerializeField] float minPointDistance = 0f;

	List<Vector3> points = new List<Vector3>();
	Util.ProximityComparer proximity;

	public override void Initialize () {
		base.Initialize ();
		points.Clear();
		proximity = new Util.ProximityComparer(minPointDistance);
	}


	protected override float Process (float value, int x, int y)
	{
		if (value == 0f) return 0;

		var point = Generator.ImageToWorld(x,y);
		if (minPointDistance <= 0) {
			// every point above the threshold is an edge
			points.Add(point);
			return value;
		}

		if (points.Contains(point, proximity)) return 0;	// point to close

		points.Add(point);
		return value;
	}

}
