using UnityEngine;

[RequireComponent (typeof (Quest))]	// This may not work allone, because you never know when youre finihsed...
public class GreedQuest : Quest {

	[Range (0,1000)]
	public int maxShotsAllowed = 500;

	private Cannons left, right;
	private int shotsFired = 0;
	private Quest other;

	public override void OnEnable () {
		base.OnEnable ();
		var player = GameObject.FindGameObjectWithTag ("Player");
		left = Cannons.OnBackboardFrom (player);
		right = Cannons.OnSteuerboardFrom (player);

		left.OnFired += OnCannonballFired;
		right.OnFired += OnCannonballFired;

		other = GetComponent<Quest> ();
		other.OnFinished += OnOtherQuestFinished;

		IsFulfilled = true;
		UpdateMenu (shotsFired, maxShotsAllowed);
	}

	public override void OnDisable () {
		base.OnDisable ();
		if (left != null) left.OnFired -= OnCannonballFired;
		if (right != null)right.OnFired -= OnCannonballFired;
		if (other != null) other.OnFinished -= OnOtherQuestFinished;
		
	}

	void OnOtherQuestFinished () {
		this.IsFinished = true;
	}

	void OnCannonballFired () {
		shotsFired++;
		if (shotsFired > maxShotsAllowed) {
			IsFulfilled = false;
			IsFinished = true;
		}
		UpdateMenu (shotsFired, maxShotsAllowed);
	}

}
