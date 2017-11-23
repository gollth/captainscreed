using UnityEngine;
using System.Collections;

public class Cannonball : MonoBehaviour {

	public float flyspeed = 0;
	public int damage = 5;
	[Range(0,2)]
	public float activateColliderAfter = 0.3f;  // seconds

    public Cannons shooter { get; set; }


	Collider2D colli;
	float size = 1;

	void Start () {
		this.colli = GetComponent<Collider2D> ();
		size = GetComponent<SpriteRenderer> ().bounds.extents.magnitude;
		Invoke ("ActivateCollider", activateColliderAfter);
	}

	void ActivateCollider () {
		colli.enabled = true;
		//Debug.DrawRay (transform.position, Vector2.down, Color.black, 1);
	}


	// Update is called once per frame
	void Update () {
		this.transform.position += transform.up * flyspeed * Time.deltaTime;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		// Ignore mid-air collisions with other cannonballs
		if (collision.gameObject.tag == "Cannonball") return;

		bool otherShip = collision.gameObject.GetComponent<Ship> () != null;

		// On every other collision, destroy something =)
		Explosion.Detonate (this.transform.position, 3 * size, mute: !otherShip);
		Destroy (this.gameObject);
	}
}
