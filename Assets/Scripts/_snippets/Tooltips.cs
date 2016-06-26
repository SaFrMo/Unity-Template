using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace SaFrLib {
	public class Tooltips : MonoBehaviour {

		public static string defaultPrefabPath = "SaFrMo/Tooltip";

		/// TOOLTIPS
		/// ===============
		/// - WHAT
		/// 	Tooltips provide UI popups to (a) CONVEY INFORMATION to the player and
		/// 	(b) ENABLE ACTIONS that the player can take.
		/// - HOW
		/// 	Call Tooltips.CreateNew with the following options:
		/// 	- string toDisplay (required) - the text to display in the Tooltip
		/// 	- RectTransform parent (optional) - the RectTransform parent of this Tooltip.
		/// 		Defaults to any Canvas in the scene and creates a Canvas if none is in place.
		/// 	- Vector3 position (optional) - the center of the spawned Tooltip
		/// 	- Transform toFollow (optional) - the Transform this tooltip will follow
		/// 	- Vector3 offset (optional, requires toFollow) - the distance between this
		/// 		Tooltip and the transform it will follow
		/// 	- TooltipCallback onCreation (optional) - callback to be fired when this	
		/// 		Tooltip is created (uses created tooltip as parameter)
		/// 	- TooltipCallback on (optional) - callback to be fired when this
		/// 		Tooltip is destroyed (uses created tooltip as parameter)
		/// 	- string exitText (optional) - text to display on 'Continue...' button (defaults
		/// 		to 'Continue')
		/// 	- string[] exitChoices - creates buttons that destroy the Tooltip and invoke
		/// 		the corresponding function in exitCallbacks (requires exitCallbacks to be
		/// 		defined)
		/// 	- TooltipCallback[] exitCallbacks - methods to invoke when the corresponding
		/// 		exitChoice button is pressed
		/// 
		/// - TODO
		/// 	- Style options
		/// 
		/// 

		// Tooltip prefab to instantiate
		public GameObject prefab;
		public delegate void TooltipCallback(GameObject createdTooltip);
	
		public static void CreateNew(string toDisplay, RectTransform parent = null, TooltipCallback onCreation = null, TooltipCallback onDestruction = null) {

			// Look for Tooltips component in the scene
			Tooltips t = FindObjectOfType<Tooltips>();
			if (t == null) {
				GameObject g = new GameObject("Tooltip Master", typeof(Tooltips));
				t = g.GetComponent<Tooltips>();
			}

			// Set prefab to instantiate
			if (t.prefab == null) {
				t.prefab = Resources.Load<GameObject>(defaultPrefabPath);
			}
			GameObject newTooltip = Instantiate<GameObject>(t.prefab) as GameObject;
			// Make sure new tooltip has Tooltip component
			if (newTooltip.GetComponent<Tooltip>() == null) {
				newTooltip.AddComponent<Tooltip>();
			}
			Tooltip tooltip = newTooltip.GetComponent<Tooltip>();

			// OnCreate callback
			if (onCreation != null) {
				onCreation.Invoke(newTooltip);
			}

			// Save OnDestruction callback
			tooltip.onDestruction = onDestruction;

			// Set parent
			if (parent == null) {
				Canvas c = FindObjectOfType<Canvas>();
				if (c == null) {
					c = new GameObject("Canvas (Created by Tooltips.cs)", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
					c.renderMode = RenderMode.ScreenSpaceOverlay;
					GameObject g = new GameObject("Event System (Created by Tooltips.cs)", typeof(EventSystem), typeof(StandaloneInputModule));
				}
				parent = c.GetComponent<RectTransform>();
			}

		}

		void Start() {
			Tooltips.CreateNew("test");
		}
	}
}