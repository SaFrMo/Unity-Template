using UnityEngine;
using System.Collections;
using SaFrLib;

public class Standard2dMover : MonoBehaviour {

	/// STANDARD 2D MOVER
	/// ==================
	/// 
	/// WHAT:
	/// 	A simple class to enable 2D movement. Imagine a Zelda-like gridless top-down sprite system.
	/// 
	/// HOW:
	/// 	Attach to a GameObject. That's it! The Standard2dMover either uses the Rigidbody2D/Rigidbody
	/// 	you've attached to the GameObject, or it creates a Rigidbody2D to use on its own.
	/// 

	//

	/// <summary>
	/// The movement speed of this GameObject.
	/// </summary>
	public float speed = 3f;
	/// <summary>
	/// Whether this GameObject smooths out its movement using Input.GetAxis or not.
	/// </summary>
	public bool smoothMovement = true;
	/// <summary>
	/// Whether this GameObject moves on the XY axes (`true`) or XZ (`false`)
	/// </summary>
	public bool useXYAxis = true;

	protected Rigidbody rigid;
	protected Rigidbody2D rigid2d;

	protected virtual void Start() {
		// Check for existing Rigidbody2D or Rigidbody
		rigid2d = GetComponent<Rigidbody2D>();
		rigid = GetComponent<Rigidbody>();

		// Create Rigidbody2D if no Rigidbody2D or Rigidbody exists
		if (rigid2d == null && rigid == null) {
			rigid2d = SaFrMo.GetOrCreate<Rigidbody2D>(gameObject);
		}
	}

	protected virtual void Move(Vector2 movement) {
		if (rigid2d) {
			rigid2d.MovePosition((Vector2)transform.position + movement);
		} else {
			Vector3 toMove = new Vector3(movement.x, useXYAxis ? movement.y : 0, useXYAxis ? 0 : movement.y);
			rigid.MovePosition(transform.position + toMove);
		}
	}

	public virtual void Update() {
		// Capture smoothed/unsmoothed input
		Vector2 input;
		if (smoothMovement) {
			input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		} else {
			input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		}

		Move(input * speed * Time.deltaTime);
	}
}
