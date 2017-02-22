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
	/// 	you've attached to the GameObject, or it creates a Rigidbody2D to use on its own. Remember to
	/// 	turn gravity on or off as desired.
	/// 

	//

	/// <summary>
	/// The movement speed of this GameObject.
	/// </summary>
	public float speed = 3f;

	/// <summary>
	/// Whether or not this Mover accepts user input (WASD).
	/// </summary>
	public bool inputEnabled = false;
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

	protected Coroutine currentCoroutine;
	public delegate void MovementCallback2d();

	protected virtual void Start() {
		// Check for existing Rigidbody2D or Rigidbody
		rigid2d = GetComponent<Rigidbody2D>();
		rigid = GetComponent<Rigidbody>();

		// Create Rigidbody2D if no Rigidbody2D or Rigidbody exists
		if (rigid2d == null && rigid == null) {
			rigid2d = SaFrMo.GetOrCreate<Rigidbody2D>(gameObject);
		}
	}

	/// <summary>
	/// Move the specified amount.
	/// </summary>
	/// <param name="movement">Movement.</param>
	protected virtual void Move(Vector2 movement) {
		if (rigid2d) {
			rigid2d.MovePosition((Vector2)transform.position + movement);
		} else {
			Vector3 toMove = new Vector3(movement.x, useXYAxis ? movement.y : 0, useXYAxis ? 0 : movement.y);
			rigid.MovePosition(transform.position + toMove);
		}
	}

	/// <summary>
	/// Moves to a specified point, firing a callback on completion.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="onComplete">On complete.</param>
	/// <param name="interruptCurrentMovement">If set to <c>true</c> interrupt current movement.</param>
	public virtual void MoveTo(Vector2 position, MovementCallback2d onComplete = null, float minDistance = 0.01f, bool interruptCurrentMovement = true) {
		// Stop current movement if desired
		if (interruptCurrentMovement && currentCoroutine != null) {
			StopAutomaticMovement ();
		}

		currentCoroutine = StartCoroutine (MoveToCoroutine (position, onComplete, minDistance));
	}

	/// <summary>
	/// Stops the automatic movement.
	/// </summary>
	public virtual void StopAutomaticMovement() {
		if (currentCoroutine != null) {
			StopCoroutine (currentCoroutine);
			currentCoroutine = null;
		}
	}

	/// <summary>
	/// Movement coroutine. Approaches a specified point, then fires a callback on completion.
	/// </summary>
	/// <returns>The to coroutine.</returns>
	/// <param name="target">Target.</param>
	/// <param name="onComplete">On complete.</param>
	/// <param name="minDistance">Minimum distance.</param>
	protected virtual IEnumerator MoveToCoroutine(Vector2 target, MovementCallback2d onComplete, float minDistance){
		while (!SaFrMo.CloseEnough2d ((Vector2)transform.position, target, minDistance)) {
			Vector2 nextPoint = Vector2.MoveTowards ((Vector2)transform.position, target, speed * Time.deltaTime);
			if (rigid2d != null) {
				rigid2d.MovePosition (nextPoint);
			} else {
				rigid.MovePosition ((Vector3)nextPoint);
			}
			yield return new WaitForEndOfFrame ();
		}

		// Fire callback if specified
		if (onComplete != null) {
			onComplete.Invoke ();
		}

		// Clean out current coroutine reference
		currentCoroutine = null;
	}

	public virtual void Update() {
		if (inputEnabled) {
			// Capture smoothed/unsmoothed input
			Vector2 input;
			if (smoothMovement) {
				input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			} else {
				input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			}

			Move (input * speed * Time.deltaTime);
		}
	}
}
