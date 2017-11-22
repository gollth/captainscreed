using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonColliderFilter : Filter {

	[SerializeField] float clusteringDistance = 1f;

	PolygonCollider2D poly;
	Util.ProximityComparer proximity;
	List<List<Vector3>> groups = new List<List<Vector3>> ();

	void OnDrawGizmosSelected ()
	{
		if (groups.Count > 100)	return;
		int i = 0;
		lock(groups) {
			foreach (var g in groups) {
				if (g.Count == 0) { Debug.LogWarning("Empty Group found!"); continue; }
				Random.InitState(i++);
				var hue = Random.Range(0f,1f);
				Gizmos.color = Color.HSVToRGB(hue, 1,1);
				var center = g.Aggregate(Util.Vector3Sum) / g.Count;
				Gizmos.DrawSphere(center, transform.lossyScale.Mean()/200);

				var path = FindLoop(g.ToList()).ToList();
				var start = path[0];
				var point = start;
				foreach (var p in path) {
					if (p == point){ point = p; continue;}
					Gizmos.color = Color.HSVToRGB(hue, 1, 1);
					Gizmos.DrawLine(point, p);
					point = p;
				}
				Gizmos.DrawLine(point, start);
			}
		}
	}

	IEnumerable<Vector3> FindLoop (List<Vector3> p) {
		
		var reference = p[0];
		yield return reference;
		p.Remove(reference);

		while (p.Count > 0) {
			var closest = p.Closest (reference);
			yield return closest;
			p.Remove (closest);
			reference = closest;
		}
	}

	public override void Initialize () {
		base.Initialize();
		groups.Clear();
		poly = GetComponent<PolygonCollider2D>();
		poly.pathCount = 0;
		proximity = new Util.ProximityComparer (clusteringDistance);
	}

	protected override float Process (float value, int x, int y) {

		if (value == 0f) return value;
		var point = Generator.ImageToWorld(x,y);

		// If this is the very first point, we just create a new group and omit further checks
		lock(groups) {
			if (groups.Count == 0) {
				var g = new List<Vector3> ();
				g.Add (point);
				groups.Add (g);
				return value;
			}

			// First we check to how many groups this point would fit ...
			var closestTo = new List<List<Vector3>> ();
			foreach (var g in groups) {
				if (g.Contains (point, proximity))
					closestTo.Add (g);
			}

			// if the current point is close to more than one group, we merge all of them into one
			if (closestTo.Count > 1) {
				var merged = new List<Vector3> ();
				foreach (var g in closestTo) {
					merged.AddRange (g);
					groups.Remove (g);
				}
				groups.Add (merged);
				// now the point will only be close to one group anymore
			}

			// Choose the right group to add the point to
			bool foundGroup = false;
			foreach (var g in groups) {
				
				// If a point is in close proximity to an existing group, we add it there ...
				if (g.Contains (point, proximity)) { 
					g.Add (point); 
					foundGroup = true; 
				}
			}

			if (foundGroup)	return value;

			// if not we create a new group and add the point to there
			var newgroup = new List<Vector3> ();
			newgroup.Add (point);
			groups.Add (newgroup);
		}
		return value;

	}

	public override void FinishMainThread ()
	{
		int i = 0;
		poly.pathCount = groups.Count;
		foreach (var g in groups) {
			poly.SetPath(i++, FindLoop(g.ToList())
							 .Select  (v2 => (Vector2)this.transform.InverseTransformPoint(v2))
						     .ToArray());
		}
	}


}
