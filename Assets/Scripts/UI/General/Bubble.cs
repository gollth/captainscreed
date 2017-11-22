using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScreenMover))]
public class Bubble : MonoBehaviour, IPointerClickHandler {


	#region Statics
	static Bubble lastBubble = null;
	public static Bubble MessageByFirstOfficer (string key) {
		var bubble = Instantiate (Resources.Load ("UI/bubble_first_officer")) as GameObject;
		var rt = bubble.transform as RectTransform;
		rt.SetParent (GameObject.Find ("UI").transform, false);

		var setter = bubble.GetComponentInChildren<LocaleStringSetter> ();
		setter.key = key;
		setter.SyncKeyAndText ();
		var b = bubble.GetComponent<Bubble> ();
		if (lastBubble != null) lastBubble.Hide ();
		lastBubble = b;
		return b;
	}
	public static Bubble MessageByBoss (GameObject ressource) {
		var bubble = Instantiate (ressource) as GameObject;
		var rt = bubble.transform as RectTransform;
		rt.SetParent (GameObject.Find ("UI").transform, true);
		rt.localScale = Vector3.one;
		var b = bubble.GetComponent<Bubble> ();
		if (lastBubble != null) lastBubble.Hide ();
		lastBubble = b;
		return b;
		
	}
	#endregion

	LocaleStringSetter setter;
	ScreenMover mover;

	void Start () {	
		setter = GetComponentInChildren<LocaleStringSetter> ();
		setter.SyncKeyAndText ();

		mover = GetComponent<ScreenMover> ();

		Show ();

	}


	public void OnPointerClick(PointerEventData e) { Hide (); }

	public void Show() { mover.Execute (); }
	public void Hide() { mover.Reverse (callback: () => Destroy(this.gameObject)); }

}

