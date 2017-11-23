using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Util {

	#region Classes
	[System.Serializable] public class UnityEventGameObject : UnityEvent <GameObject> {}
	#endregion

	public static float unscaledDeltaTime { get { return Mathf.Clamp (Time.unscaledDeltaTime, 0, .3f); } }

    #region Vector Algebra
    public class ProximityComparer : IEqualityComparer<Vector3> {
    	public float Threshold { get; set; }
    	public ProximityComparer(float threshold) { this.Threshold = threshold; }

    	public bool Equals (Vector3 a, Vector3 b) {
    		return (a-b).sqrMagnitude <= Threshold * Threshold;
		}

		public int GetHashCode (Vector3 v) {
			return v.GetHashCode();
		}

    }

	/// <summary>
    ///     ClockwiseComparer provides functionality for sorting a collection of Vector2s such
    ///     that they are ordered clockwise about a given origin.
    /// </summary>
    public class ClockwiseComparer : IComparer<Vector2>
    {
            private Vector2 m_Origin;

            #region Properties

            /// <summary>
            ///     Gets or sets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }

            #endregion

            /// <summary>
            ///     Initializes a new instance of the ClockwiseComparer class.
            /// </summary>
            /// <param name="origin">Origin.</param>
            public ClockwiseComparer(Vector2 origin)
            {
                    m_Origin = origin;
            }

            #region IComparer Methods

            /// <summary>
            ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="first">First.</param>
            /// <param name="second">Second.</param>
            public int Compare(Vector2 first, Vector2 second)
            {
                    return IsClockwise(first, second, m_Origin);
            }

            #endregion

            /// <summary>
            ///     Returns 1 if first comes before second in clockwise order.
            ///     Returns -1 if second comes before first.
            ///     Returns 0 if the points are identical.
            /// </summary>
            /// <param name="first">First.</param>
            /// <param name="second">Second.</param>
            /// <param name="origin">Origin.</param>
            public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
            {
                    if (first == second)
                            return 0;

                    Vector2 firstOffset = first - origin;
                    Vector2 secondOffset = second - origin;

                    float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
                    float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

                    if (angle1 < angle2)
                            return -1;

                    if (angle1 > angle2)
                            return 1;

                    // Check to see which point is closest
                    return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? -1 : 1;
            }
    }

    public static float Mod360 (float angle) {
		angle = angle % 360;
		while (angle < 0)
			angle += 360;
		return angle;
	}
	public static float Mod180 (float angle) {
		angle = angle % 360;
		while (angle < -180)
			angle += 360;
		while (angle > 180)
			angle -= 360;

		return angle;
	}
    public static float Direction2ZAngle(Vector3 direction) {
        return Mathf.Rad2Deg * -Mathf.Atan2(direction.y, direction.x) + 90;
    }
    public static Vector3 ZAngle2Direction(float angle){
        return new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * angle),
            Mathf.Cos(Mathf.Deg2Rad * angle),
            0
        );
    }
	public static int ZAngle2Clock (float angle) {
    	return Mathf.RoundToInt (12 * Mod360 (angle) / 360);
	}
	public static float Direction2NEWS (Vector3 direction) {
		return Mathf.Round(Direction2ZAngle(direction) / 90) * 90;
	}
	public static Rect Scale(this Rect rect, float scale){
        var newSize = rect.size * scale;
        return new Rect(-newSize - rect.min, 2 * newSize);
    }
	public static Vector2 ScaleEach (this Vector2 me, Vector2 other) {
		return Vector2.Scale (me, other);
	}
	public static Vector3 ScaleEach (this Vector3 me, Vector3 other) {
		return Vector3.Scale (me, other);
	}
	public static Vector2 Inverse (this Vector2 v) {
		return new Vector2 (1f / v.x, 1f / v.y);
	}
	public static Vector3 Inverse (this Vector3 v) {
		return new Vector3(1f/v.x, 1f/v.y, 1f/v.z);
	}
	public static bool Is(this Vector3 v, System.Func<float, bool> predicate, bool any=true) {
		if (any) return predicate (v.x) || predicate (v.y) || predicate (v.z);
		else return predicate (v.x) && predicate (v.y) && predicate (v.z);
	}
	public static bool Is(this Vector2 v, System.Func<float, bool> predicate, bool any=true) {
		if (any) return predicate (v.x) || predicate (v.y);
		else return predicate (v.x) && predicate (v.y);
	}
	public static Vector2 Each(this Vector2 v, System.Func<float, float> applier) {
		var w = new Vector2 ();
		w.x = applier (v.x);
		w.y = applier (v.y);
		return w;
	}
	public static Vector3 Each(this Vector3 v, System.Func<float, float> applier) {
		var w = new Vector3 ();
		w.x = applier (v.x);
		w.y = applier (v.y);
		w.z = applier (v.z);
		return w;
	}

	public static Vector3 Closest (this IEnumerable<Vector3> candidates, Vector3 reference) {
		if (candidates.Count() == 0) throw new System.ArgumentException("Candidates list is empty!");
		return candidates.OrderBy(p => (p - reference).sqrMagnitude).First();
	}

	public static Color AsColor(this Vector2 v) { return new Color (v.x, v.y, 0); }
	public static Color AsColor(this Vector3 v) { return new Color (v.x, v.y, v.z); }

	public static Vector2 Flip(this Vector2 v) { return new Vector2 (v.y, v.x); }
	public static Vector3 Flip(this Vector3 v) { return new Vector3 (v.y, v.x, v.z); }

	public static float Mean(this Vector2 v) { return (v.x + v.y) / 2f; }
	public static float Mean(this Vector3 v) { return (v.x + v.y + v.z) / 3f; }

	public static Vector2 Sin(Vector2 v) {
		return new Vector2 (Mathf.Sin (v.x), Mathf.Sin (v.y));
	}
	public static Vector3 Sin(Vector3 v) {
		return new Vector3 (Mathf.Sin (v.x), Mathf.Sin (v.y), Mathf.Sin(v.z));
	}
	public static Vector2 Cos(Vector2 v) {
		return new Vector2 (Mathf.Cos (v.x), Mathf.Cos (v.y));
	}
	public static Vector3 Cos(Vector3 v) {
		return new Vector3 (Mathf.Cos (v.x), Mathf.Cos (v.y), Mathf.Cos(v.z));
	}
	public static Vector2 Tan(Vector2 v) {
		return new Vector2 (Mathf.Tan (v.x), Mathf.Tan (v.y));
	}
	public static Vector3 Tan(Vector3 v) {
		return new Vector3 (Mathf.Tan (v.x), Mathf.Tan (v.y), Mathf.Tan(v.z));
	}

    #endregion


    #region Numbers
    public static bool Inside (float value, float min, float max) {
		return min <= value && value <= max;
	}
	public static bool Outside (float value, float min, float max) {
		return !Inside (value, min, max);
	}
	public static bool Around (float value, float target, float threshold) {
		return Mathf.Abs (value - target) <= threshold; 
	}
	public static float dB (this float x) { return 20 * Mathf.Log10(x); }
	public static float linear (this float dB) { return Mathf.Pow(10, dB/20); }
    #endregion


    #region Strings
    public static bool OneInTheOther (string one, string other) {
		return one.Contains (other) || other.Contains (one);
	}
    public static string GetLocaleFromLanguageName(string name){
        switch (name)
        {
            case "English": return "en";
            case "German": return "de";
            default: return "en";
        }
    }
    public static int CountPlaceholders (string format) {
		if (string.IsNullOrEmpty (format)) return 0;
		return Regex.Matches(format, @"(?<!\{)\{([0-9]+).*?\}(?!})")
     		.Cast<Match>()
     		.DefaultIfEmpty()
     		.Max(m => m==null?-1:int.Parse(m.Groups[1].Value)) + 1;
    }
	public static int GetLevelFromScene (string name) {
		var c = char.ToUpper(name[0]);
		return c - 64;
	}
    #endregion


    #region Debugging
    public static void DrawArrow (Vector3 start, Vector3 direction, Color color = new Color()) {
		DrawArrow (start, Direction2ZAngle (direction), direction.magnitude, color);
	}
	public static void DrawArrow (Vector3 start, float z, float length, Color color = new Color()) {
		Vector3 direction = ZAngle2Direction (z);
		Vector3 end = start + length * direction;
		Debug.DrawLine (start, end, color);
		Debug.DrawLine (end, end + 0.3f * length * ZAngle2Direction (z + 180 - 15), color);
		Debug.DrawLine (end, end + 0.3f * length * ZAngle2Direction (z + 180 + 15), color);
	}
    #endregion


    #region Referencing
    public static List<GameObject> GetAllGameObjects(string layer = null) {
		return GetAllGameObjects (new string[] { layer });
	}
	public static List<GameObject> GetAllGameObjects(string[] layers = null) {
		var objects = MonoBehaviour.FindObjectsOfType<GameObject> ();
		// Map layer names to ints, check whos objects matches any of those layer ints
		if (layers != null)	objects = objects
			.Where (item => layers
				.Select (name => LayerMask.NameToLayer(name))
				.Contains(item.layer))
			.ToArray();
		return objects.ToList();
	}

	public static RectTransform CreateUIElement (GameObject prefab, Vector2 viewportPosition, Vector2 size, string parentName="UI") {
		var o = MonoBehaviour.Instantiate (prefab, 
			Vector3.zero,
			Quaternion.identity) as GameObject;

		var rt = o.transform as RectTransform;
		rt.anchorMin = viewportPosition;
		rt.anchorMax = viewportPosition;
		rt.sizeDelta = Vector2.Scale (size, new Vector2 (Screen.width, Screen.height));
		rt.SetParent (GameObject.Find (parentName).transform, false);
		return rt;
	}

	public static T GetComponentInSibling <T>(this Transform t) where T : Component {
		return t.parent.GetComponentInChildren<T> ();
	}
	public static IEnumerable<T> GetComponentsInChildrenWithoutSelf <T> (this Transform t) where T : Component{
         return t.GetComponentsInChildren<T>().Where(comp => comp.gameObject.GetInstanceID() != t.gameObject.GetInstanceID());
    }
	public static T GetComponentInChildrenWithoutSelf <T> (this Transform t) where T : Component{
         return t.GetComponentsInChildrenWithoutSelf<T>().FirstOrDefault();
    }
    #endregion

    #region Linq Stuff
    public static System.Func<T, bool> BelongsTo<T> (GameObject g) where T : Component {
    	return c => c.gameObject.GetInstanceID() == g.GetInstanceID();
    }
    public static System.Func<T, bool> Not<T> (this System.Func<T, bool> f) { return x => !f(x); }

    public static System.Func<T, bool> Enabled<T>() where T : MonoBehaviour { 
      	return (T c) => c.enabled;
    }

    public static readonly System.Func<Vector3, Vector3, Vector3> Vector3Sum = (sum, v) => sum + v;
    public static readonly System.Func<Vector2, Vector2, Vector3> Vector2Sum = (sum, v) => sum + v;
    #endregion

    #region Camera
    public static bool IsVisibleToCamera (Vector2 point, Camera camera=null) {
		if (camera == null) camera = Camera.main;
		var p = camera.WorldToViewportPoint (point);
		return new Rect(Vector2.zero, Vector2.one).Contains (p);
	}
	public static Rect ViewportRect { get { return new Rect (
		position: Camera.main.ViewportToWorldPoint (new Vector3 (.5f,.5f,0)),
		size: Camera.main.ViewportToWorldPoint (Vector3.right + Vector3.up)
	);}}
	public static Vector2 ScreenSize {
		get { 
			return (Vector2)Camera.main.transform.InverseTransformVector (
		   		    Camera.main.ViewportToWorldPoint(new Vector3(1,1,0))
				-Camera.main.ViewportToWorldPoint(Vector3.zero));
		}
	}
	public static Vector2 ScreenSizePx { get { 
		return (Vector2)Camera.main.WorldToScreenPoint(ScreenSize) / 1.5f;	// TODO find out why 1.5 ?!?
	} }

	#endregion

}
