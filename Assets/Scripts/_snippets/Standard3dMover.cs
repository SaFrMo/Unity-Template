using UnityEngine;
using System.Collections;
using SaFrLib;

public class Standard3dMover : MonoBehaviour {

	/// STANDARD 3D MOVER
	/// ==================
	/// 
	/// WHAT:
	/// 	A simple class to enable 3D movement, including forward/backward, turning, and jumping.
	/// 
	/// HOW:
	/// 	Attach to a GameObject with some kind of Collider. That's it!
	/// 

	//

	protected Rigidbody rigid;
	/// <summary>
	/// The forward/backward speed.
	/// </summary>
	public float speed = 1f;
	/// <summary>
	/// The Y axis rotation speed.
	/// </summary>
	public float rotationSpeed = 45f;
	/// <summary>
	/// The jump force. Applied as ForceMode.Impulse during Update.
	/// </summary>
	public float jumpForce = 5f;

	protected virtual void Start() {
		rigid = SaFrMo.GetOrCreate<Rigidbody> (gameObject);
	}

	protected virtual void FixedUpdate() {
		// Capture input
		Vector2 input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		// Move forward
		Vector3 newPos = transform.position;
		newPos += transform.forward * input.y * speed * Time.fixedDeltaTime;
		rigid.MovePosition (newPos);

		// Rotate
		transform.Rotate(new Vector3(0, input.x * rotationSpeed * Time.fixedDeltaTime, 0));
	}

	protected virtual void Update() {
		// Jump
		if (Input.GetKeyDown (KeyCode.Space)) {
			rigid.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
		}
	}
}
