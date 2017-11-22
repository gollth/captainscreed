using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;
using System.Collections;


public class Quartermaster : MonoBehaviour, IPointerDownHandler, 
											IDragHandler, 
											IEndDragHandler, 
											IPointerUpHandler {
	#region Tweakables
	public Animator animator;
	[Range(0,1), Tooltip("The higher the value, the more you have to drag to the edge of the menu, to apply full scale")]
	public float circleRadius = 1;
	public Text thrust;
	public Text reload;
	public Text repair;

	public RectTransform menu;
	public RectTransform menuTopLeft, menuBottomRight;

	#endregion

	#region References

	private Crew crew;
	private LocaleStringSetter lssThrust, lssReload, lssRepair;

	private int layer;
	private AnimatorClipInfo[] infos;
	#endregion

	Vector2 MenuViewportSize { get { 
			var size = Camera.main.WorldToViewportPoint (menuBottomRight.position) - Camera.main.WorldToViewportPoint (menuTopLeft.position);
			size.y *= -1;
			return size;
		} }
	


	void Start () {
		this.crew = GetComponentInParent<Crew> ();
	
		lssThrust = thrust.GetComponent<LocaleStringSetter> ();
		lssReload = reload.GetComponent<LocaleStringSetter> ();
		lssRepair = repair.GetComponent<LocaleStringSetter> ();

		layer = animator.GetLayerIndex ("Selection");
		if (layer == -1) Debug.LogError ("Cannot find layer \"Selection\" of Animator " + this.name);

		circleRadius = GetComponentInParent<ScreenSizer>().size.x * Util.ScreenSize.x / 2;

		// Cache the initial clip infos (all equal weight)
		infos = animator.GetCurrentAnimatorClipInfo (layer);
		OnEndDrag (null);	// Call once for initialization of Crew distribution


	}
	
	// Update is called once per frame
	void Update () {
		// Only process blending, if selection wheel is visible
		if (!animator.GetBool ("touch")) return;

		// Update the weights
		var currentInfo = animator.GetCurrentAnimatorClipInfo (layer);
		for (int i = 0; i < 3; i++) {
			// Find the element in the current infos, which name matches the old one ...
			var update = currentInfo.SingleOrDefault (item => item.clip.name.Equals (infos [i].clip.name));
			// if not null (default for struct), update
			if (!update.Equals (default (AnimatorClipInfo))) infos [i] = update;
		}

		crew.ObeyOrder (infos [0].weight, infos [1].weight, infos [2].weight, false);

		// Set the UI-Texts accordingly
		lssThrust.SetValues (crew.Thrusters);
		lssReload.SetValues (crew.Reloaders);
		lssRepair.SetValues (crew.Repairers);
	}

	public void OnPointerDown(PointerEventData ev) {
		if (crew.amount == 0 || !crew.enabled) return;
		// Show the animator and cache the ship's position
		animator.SetBool ("touch", true);
	}

	public void OnDrag (PointerEventData ev) {
		if (crew.amount == 0|| !crew.enabled) return;

		// Check, how much relative motion the user made
		Vector3 motion = Camera.main.transform.InverseTransformVector(Camera.main.ScreenToWorldPoint(ev.position) - Camera.main.ScreenToWorldPoint(ev.pressPosition));
		motion /= circleRadius;
		motion *= 2f;

		// Set the animator accordingly
		animator.SetFloat ("X", motion.x);
		animator.SetFloat ("Y", motion.y);
	}

	public void OnPointerUp (PointerEventData ev) {
		if (crew.amount == 0|| !crew.enabled) return;
		HideMenu (false);
	}

	public void OnEndDrag (PointerEventData ev) {
		if (crew.amount == 0|| !crew.enabled) return;
		HideMenu (true);
	}

	void HideMenu (bool applyChanges) {
		// Hide the menu
		animator.SetBool ("touch", false);
		if (!applyChanges) return;

		// Tell the crew the changes
		crew.ObeyOrder (infos[0].weight, infos[1].weight, infos[2].weight);
	}

}
