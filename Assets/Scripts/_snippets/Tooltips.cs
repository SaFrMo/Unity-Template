using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace SaFrLib {
	public class Tooltips : MonoBehaviour {

		public static string defaultPrefabPath = "SaFrLib/Default Tooltip";

		/// TOOLTIPS
		/// ===============
		/// - WHAT
		/// 	Tooltips provide UI popups to (a) CONVEY INFORMATION to the player and
		/// 	(b) ENABLE ACTIONS that the player can take.
		/// - HOW
		/// 	Call Tooltips.CreateNew with the following options:
		/// 	- string toDisplay (required) - the text to display in the Tooltip
		/// 	- RectTransform parent (optional) - the RectTransform parent of this Tooltip.
		/// 		Defaults to the first Canvas in the scene and creates a Canvas if none is in place.
		/// 	- Vector3 position (optional) - the center of the spawned Tooltip
		/// 	- Transform toFollow (optional) - the Transform this tooltip will follow
		/// 	- Vector3 offset (optional, requires toFollow) - the distance between this
		/// 		Tooltip and the transform it will follow
		/// 	- TooltipCallback onCreation (optional) - callback to be fired when this	
		/// 		Tooltip is created (uses created tooltip as parameter)
		/// 	- TooltipCallback onDestruction (optional) - callback to be fired when this
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
		/// 	- Style options: background color, font color, font family
		/// 	- Headline on tooltip
		/// 

		// Tooltip prefab to instantiate
		public GameObject prefab;
		public delegate void TooltipCallback(GameObject createdTooltip);
	
		public static Tooltip CreateNew(string toDisplay, RectTransform parent = null, Vector3 position = default(Vector3), 
										Transform toFollow = null, Vector3 offset = default(Vector3), 
										string exitText = "Continue", string[] exitChoices = null, TooltipCallback[] exitCallbacks = null, 
										TooltipCallback onCreation = null, TooltipCallback onDestruction = null,
										TooltipStyleOptions style = null) {

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
			// Instantiate tooltip
			GameObject newTooltip = Instantiate<GameObject>(t.prefab) as GameObject;
			// Make sure new tooltip has Tooltip component
			if (newTooltip.GetComponent<Tooltip>() == null) {
				newTooltip.AddComponent<Tooltip>();
			}
			Tooltip tooltip = newTooltip.GetComponent<Tooltip>();
			RectTransform tooltipRecTransform = newTooltip.GetComponent<RectTransform>();

			// OnCreate callback
			if (onCreation != null) {
				onCreation.Invoke(newTooltip);
			}

			// Set text to display
			tooltip.SetDisplayText(toDisplay);

			// Save OnDestruction callback
			tooltip.onDestruction = onDestruction;

			// Set parent
			if (parent == null) {
				Canvas c = FindObjectOfType<Canvas>();
				if (c == null) {
					// Create Canvas and EventSystem if none exists yet
					c = new GameObject("Canvas (Created by Tooltips.cs)", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
					c.renderMode = RenderMode.ScreenSpaceOverlay;
					GameObject g = new GameObject("Event System (Created by Tooltips.cs)", typeof(EventSystem), typeof(StandaloneInputModule));
				}
				parent = c.GetComponent<RectTransform>();
			}
			newTooltip.transform.SetParent(parent, false);

			// Position new Tooltip
			// First, account for default position
			if (position == default(Vector3)) {
				position.x = Screen.width / 2f;
				position.y = Screen.height / 2f;
			}
			// If toFollow is specified, override default position
			if (toFollow != null) {
				position = Camera.main.WorldToScreenPoint(toFollow.position);
				// Move by offset
				if (offset == default(Vector3)) offset = Vector3.zero;
				position += offset;
				// Save to Tooltip
				tooltip.toFollow = toFollow;
				tooltip.toFollowOffset = offset;
			}
			tooltipRecTransform.position = position;

			// Create exit buttons
			if (exitChoices == null) {
				exitChoices = new string[] { exitText };
			}
			tooltip.CreateExitButtons(exitChoices, exitCallbacks);

			// Set style
			tooltip.style = style;

			// Return instantiated Tooltip
			return tooltip;
		}

		// Testing purposes
		GameObject g;
		void Start() {
			g = GameObject.CreatePrimitive(PrimitiveType.Cube);
			TooltipStyleOptions style = new TooltipStyleOptions(
				viewportPadding: new Vector3(20f, 0),
				draggable: true
			);
			//style.viewportPadding 
			Tooltips.CreateNew("test", 
				exitChoices: new string[] { "Exit now!", "Exit and say goodbye!" }, 
				exitCallbacks: new TooltipCallback[] { 
					x => { print("Exiting..."); },
					x => { print("Saying goodbye and exiting..."); }
				}, 
				//toFollow: g.transform,
				offset: new Vector3(30f, 1f),
				style: style
			);
		}

		void Update() {
			g.transform.position -= Vector3.up * Time.deltaTime;
			g.transform.position += Vector3.left * Time.deltaTime;
		}


	}

	public class TooltipStyleOptions {
		public TooltipStyleOptions(Vector3 viewportPadding = default(Vector3), bool draggable = false) {
			this.viewportPadding = viewportPadding;
			this.draggable = draggable;
		}

		public Vector3 viewportPadding;
		public bool draggable;
	}
}