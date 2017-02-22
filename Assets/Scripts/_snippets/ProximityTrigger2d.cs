using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using SaFrLib;

namespace SaFrLib {
	public class ProximityTrigger2d : MonoBehaviour {

		/// PROXIMITY TRIGGER 2D
		/// ===============
		/// - WHAT
		/// 	An self-contained and easily extendable way to automatically trigger a UnityEvent
		/// 	on trigger enter. Same as the ProximityTrigger, just for 2D environments!
		/// - HOW
		/// 	1. Attach to any GameObject.
		/// 	2. Set onTrigger to the appropriate callback. onTrigger requires a ProximityTrigger2d and GameObject as parameters.
		/// 	3. (Optional) Create a Collider and attach it to the same object as the ProximityTrigger2d for fine-tuned
		/// 		control over the trigger area.
		/// 

		//

		// Custom callback type
		[System.Serializable]
		public class ProximityCallback2d : UnityEvent<ProximityTrigger2D, GameObject> {}

		// Prep the callback
		[SerializeField]
		public ProximityCallback2d triggerEnter = new ProximityCallback2d();
		[SerializeField]
		public ProximityCallback2d triggerExit = new ProximityCallback2d();

		protected virtual void Start() {
			// Get or create the collider
			Collider2D c = GetComponent<Collider2D>();

			if (c == null) {
				c = gameObject.AddComponent<CircleCollider2D>();
			}

			// Make sure the collider is a trigger
			c.isTrigger = true;
		}

		// Invoke triggerEnter
		protected virtual void OnTriggerEnter2D(Collider2D coll) {
			if (triggerEnter != null) {
				triggerEnter.Invoke(this, coll.gameObject);
			}
		}

		// Invoke triggerExit
		protected virtual void OnTriggerExit2D(Collider2D coll) {
			if (triggerExit != null) {
				triggerExit.Invoke(this, coll.gameObject);
			}
		}
	}
}
