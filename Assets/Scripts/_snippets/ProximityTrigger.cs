﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using SaFrLib;

namespace SaFrLib {
	public class ProximityTrigger : MonoBehaviour {

		/// PROXIMITY TRIGGER
		/// ===============
		/// - WHAT
		/// 	An self-contained and easily extendable way to automatically trigger a UnityEvent
		/// 	on trigger enter.
		/// - HOW
		/// 	1. Attach to any GameObject.
		/// 	2. Set onTrigger to the appropriate callback. onTrigger requires a ProximityTrigger and GameObject as parameters.
		/// 	3. (Optional) Create a Collider and attach it to the same object as the ProximityTrigger for fine-tuned
		/// 		control over the trigger area.
		/// 

		//

		// Custom callback type
		[System.Serializable]
		public class ProximityCallback : UnityEvent<ProximityTrigger, GameObject> {}

		// Prep the callback
		[SerializeField]
		public ProximityCallback triggerEnter;
		[SerializeField]
		public ProximityCallback triggerExit;

		protected virtual void Start() {
			// Get or create the collider
			Collider c = GetComponent<Collider>();

			if (c == null) {
				c = gameObject.AddComponent<SphereCollider>();
			}

			// Make sure the collider is a trigger
			c.isTrigger = true;
		}

		// Invoke triggerEnter
		protected virtual void OnTriggerEnter(Collider coll) {
			if (triggerEnter != null) {
				triggerEnter.Invoke(this, coll.gameObject);
			}
		}

		// Invoke triggerExit
		protected virtual void OnTriggerExit(Collider coll) {
			if (triggerExit != null) {
				triggerExit.Invoke(this, coll.gameObject);
			}
		}
	}
}
