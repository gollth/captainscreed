using UnityEngine;

[ExecuteInEditMode, RequireComponent (typeof (SpringJoint2D))]
public class Cushion : MonoBehaviour {

	[SerializeField] Transform steerwheel;
	[SerializeField] Transform steerwheelMiddle;
	[SerializeField, Range(0,1)] float offset = 1;

	SpringJoint2D spring;

	void OnEnable() {
		spring = GetComponent<SpringJoint2D>();

		transform.position = A;
		spring.connectedAnchor = B;
		spring.distance = .8f * Vector3.Distance (transform.position, steerwheel.position)/2;
		spring.enabled = true;
	}
	
	void OnDisable () {
		spring.enabled = false;
	}	

	Vector3 A { get {
		var radius = Vector3.Distance(steerwheel.position, steerwheelMiddle.position);
		return steerwheelMiddle.position - transform.root.up * offset * radius;
	}}
	Vector3 B { get { return steerwheelMiddle.localPosition; } }
}
