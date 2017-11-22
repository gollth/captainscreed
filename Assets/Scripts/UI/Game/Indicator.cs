using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {

	public static Indicator CreateFor (Transform target, bool friendly) {
		var name = "UI/indicator_" + (friendly ? "blue" : "red");
		var i = Instantiate (Resources.Load (name)) as GameObject;
		var ind = i.GetComponent<Indicator> ();
		ind.target = target;
		i.transform.SetParent (GameObject.FindGameObjectWithTag("UI").transform.Find("indicators"), false);
		i.transform.localScale = Vector3.one;
		return ind;
	}

	public Transform target;
	[SerializeField] Image direction;
	[SerializeField] Vector2 offset = Vector2.one;

	Image image;
	bool visible = false;
	RectTransform container;
	PlayerFollower follower;

	// Use this for initialization
	void Start () {
		image = GetComponent<Image> ();
		container = transform.parent as RectTransform;
		follower = Camera.main.GetComponent<PlayerFollower>();
	}

	void OnDrawGizmosSelected () {
		if (container == null || target == null) return;
		var ta = container.InverseTransformPoint(target.transform.position);
		var originalCorners = new Vector3[4];

		container.GetWorldCorners (originalCorners);
		originalCorners = originalCorners.Select(container.InverseTransformPoint).ToArray();
		var rect  = Rect.MinMaxRect(originalCorners[0].x, originalCorners[0].y, originalCorners[2].x, originalCorners[2].y);
		var t = ta.ScaleEach((Vector3)offset);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube((Vector3)rect.center, (Vector3)rect.size);
		Gizmos.DrawSphere(t, 50f);
	}

	void Update() {
		if (target == null) { Destroy(this.gameObject); return; }

		var t = container.InverseTransformPoint(target.transform.position);
		var originalCorners = new Vector3[4];

		container.GetWorldCorners (originalCorners);
		originalCorners = originalCorners.Select(container.InverseTransformPoint).ToArray();
		var rect  = Rect.MinMaxRect(originalCorners[0].x, originalCorners[0].y, originalCorners[2].x, originalCorners[2].y);


		var becameVisible = rect.Contains(t.ScaleEach((Vector3)offset));
		if (!visible && becameVisible) follower.Engage(target.gameObject);
		visible = becameVisible;
		image.enabled = !visible;
		direction.enabled = !visible;
		if (visible) return;

		Vector2 point = Vector2.zero;
		if (!LineRectIntersection (container.root.position, t, rect, ref point)) {
			image.enabled = !visible;
			direction.enabled = !visible;
			return;
		}

		float heading = 0;
		if (Mathf.Approximately(point.x, originalCorners [2].x)) heading =-90;	// east
		if (Mathf.Approximately(point.x, originalCorners [0].x)) heading = 90;	// west
		if (Mathf.Approximately(point.y, originalCorners [2].y)) heading =  0;	// north
		if (Mathf.Approximately(point.y, originalCorners [0].y)) heading =180;	// south

		transform.localPosition = point;
		direction.transform.localRotation = Quaternion.Euler (0, 0, heading);

	}

	static bool LineRectIntersection(Vector2 lineStartPoint, Vector2 lineEndPoint, Rect rectangle, ref Vector2 result)
	{
		Vector2 minXLinePoint = lineStartPoint.x <= lineEndPoint.x ? lineStartPoint : lineEndPoint;
		Vector2 maxXLinePoint = lineStartPoint.x <= lineEndPoint.x ? lineEndPoint : lineStartPoint;
		Vector2 minYLinePoint = lineStartPoint.y <= lineEndPoint.y ? lineStartPoint : lineEndPoint;
		Vector2 maxYLinePoint = lineStartPoint.y <= lineEndPoint.y ? lineEndPoint : lineStartPoint;

		double rectMaxX = rectangle.xMax;
		double rectMinX = rectangle.xMin;
		double rectMaxY = rectangle.yMax;
		double rectMinY = rectangle.yMin;

		if (minXLinePoint.x <= rectMaxX && rectMaxX <= maxXLinePoint.x)
		{
			double m = (maxXLinePoint.y - minXLinePoint.y) / (maxXLinePoint.x - minXLinePoint.x);

			double intersectionY = ((rectMaxX - minXLinePoint.x) * m) + minXLinePoint.y;

			if (minYLinePoint.y <= intersectionY && intersectionY <= maxYLinePoint.y
				&& rectMinY <= intersectionY && intersectionY <= rectMaxY)
			{
				result = new Vector2((float)rectMaxX, (float)intersectionY);

				return true;
			}
		}

		if (minXLinePoint.x <= rectMinX && rectMinX <= maxXLinePoint.x)
		{
			double m = (maxXLinePoint.y - minXLinePoint.y) / (maxXLinePoint.x - minXLinePoint.x);

			double intersectionY = ((rectMinX - minXLinePoint.x) * m) + minXLinePoint.y;

			if (minYLinePoint.y <= intersectionY && intersectionY <= maxYLinePoint.y
				&& rectMinY <= intersectionY && intersectionY <= rectMaxY)
			{
				result = new Vector2((float)rectMinX, (float)intersectionY);

				return true;
			}
		}

		if (minYLinePoint.y <= rectMaxY && rectMaxY <= maxYLinePoint.y)
		{
			double rm = (maxYLinePoint.x - minYLinePoint.x) / (maxYLinePoint.y - minYLinePoint.y);

			double intersectionX = ((rectMaxY - minYLinePoint.y) * rm) + minYLinePoint.x;

			if (minXLinePoint.x <= intersectionX && intersectionX <= maxXLinePoint.x
				&& rectMinX <= intersectionX && intersectionX <= rectMaxX)
			{
				result = new Vector2((float)intersectionX, (float)rectMaxY);

				return true;
			}
		}

		if (minYLinePoint.y <= rectMinY && rectMinY <= maxYLinePoint.y)
		{
			double rm = (maxYLinePoint.x - minYLinePoint.x) / (maxYLinePoint.y - minYLinePoint.y);

			double intersectionX = ((rectMinY - minYLinePoint.y) * rm) + minYLinePoint.x;

			if (minXLinePoint.x <= intersectionX && intersectionX <= maxXLinePoint.x
				&& rectMinX <= intersectionX && intersectionX <= rectMaxX)
			{
				result = new Vector2((float)intersectionX, (float)rectMinY);

				return true;
			}
		}

		return false;
	}
}
