using UnityEngine;
using System.Collections;

public class SwingScript : MonoBehaviour {

    const int PLAYER_LAYER = 8;
    const int ENEMY_LAYER = 9;
    const int GRAPPLE_LAYER = 11;

    public GameObject anchor;
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
    public float swingSpeed = 10;
    public Sprite jumping;
    public Sprite standing;
	private HingeJoint2D joint;
	private HingeJoint2D anchorJoint;
	private LineRenderer rope;
    private Collider2D feetCollider;
    private SpriteRenderer renderer;
    private enum PlayerState {Standing, Jumping};
    private PlayerState playerState = PlayerState.Jumping;

	void Start () {
		rope = this.GetComponent<LineRenderer> ();
		rope.material = new Material(Shader.Find("Particles/Additive"));
		rope.SetColors(c1, c2);
		rope.SetWidth(0.2F, 0.2F);
		rope.SetVertexCount(2);
		rope.enabled = false;
        feetCollider = transform.Find("feet_collider").GetComponent<BoxCollider2D>();
        renderer = this.GetComponent<SpriteRenderer>();
	}

	void StartSwing(Vector3 pos) {
		/** On left Click **/
		var layerMask = 1 << GRAPPLE_LAYER;
		Vector3 currentPos = gameObject.transform.position;
		RaycastHit2D hit = Physics2D.Raycast (currentPos, pos - currentPos, Mathf.Infinity, layerMask);
		if (hit.collider != null) {
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
		}
	}

	void StopSwing () {
		//Destroy HingeJoints
		rope.enabled = false;
		Destroy (joint);
		Destroy (anchorJoint);
	}

    bool IsRopeCut()
    {
        if (!rope.enabled)
        {
            return false;
        }
        var dirVector = anchor.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, dirVector.magnitude, 1 << ENEMY_LAYER);
        return hit.collider != null;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            if (playerState == PlayerState.Standing)
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 300);
            }
            else {
                StartSwing(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        } else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || IsRopeCut()) {
            StopSwing();
        } 
        if (rope.enabled) {
			rope.SetPosition (0, transform.position);
			rope.SetPosition (1, anchor.transform.position);
        }
	}

    void FixedUpdate () {
        if (rope.enabled)
        {
            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.right * swingSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.left * swingSpeed);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerState = PlayerState.Standing;
        renderer.sprite = standing;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        playerState = PlayerState.Jumping;
        renderer.sprite = jumping;
    }
}
