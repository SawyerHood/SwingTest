using UnityEngine;
using System.Collections;

public class SwingScript : MonoBehaviour {

	public GameObject anchor;
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	private HingeJoint2D joint;
	private HingeJoint2D anchorJoint;
	private LineRenderer rope;

	void Start () {
		rope = this.GetComponent<LineRenderer> ();
		rope.material = new Material(Shader.Find("Particles/Additive"));
		rope.SetColors(c1, c2);
		rope.SetWidth(0.2F, 0.2F);
		rope.SetVertexCount(2);
		rope.enabled = false;
	}

	void StartSwing(Vector3 pos, Vector3 force) {
		/** On left Click **/
		var layerMask = ~(1 << 8);
		Vector3 currentPos = gameObject.transform.position;
		RaycastHit2D hit = Physics2D.Raycast (currentPos, pos - currentPos, Mathf.Infinity, layerMask);
		if (hit != null) {
			Vector3 collPos = hit.point;
			//move the anchor to the correct position
			anchor.transform.position = new Vector3 (collPos.x, collPos.y, 0);
			//zero out any rotation
			anchor.transform.rotation = Quaternion.identity;

			//Create HingeJoints
			joint = gameObject.AddComponent<HingeJoint2D> ();
			joint.anchor = Vector3.zero;
			joint.connectedBody = anchor.GetComponent<Rigidbody2D>();
			anchorJoint = anchor.AddComponent<HingeJoint2D> ();
			anchorJoint.anchor = Vector3.zero;

			rope.enabled = true;

			gameObject.GetComponent<Rigidbody2D>().AddForce(force);
			
		}
	}

	void StopSwing () {
		//Destroy HingeJoints
		rope.enabled = false;
		Destroy (joint);
		Destroy (anchorJoint);
	}

	void SwingRight(Vector3 pos) {
		StartSwing (pos, Vector3.right * 1000);
	}

	void SwingLeft(Vector3 pos) {
		StartSwing (pos, Vector3.left * 1000);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			SwingRight (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		} else if (Input.GetMouseButtonDown (1)) {
			SwingLeft (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		} else if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp(1)) {
			StopSwing ();
		}

		if (rope.enabled) {
			rope.SetPosition (0, transform.position);
			rope.SetPosition (1, anchor.transform.position);
		}
	
	}
}
