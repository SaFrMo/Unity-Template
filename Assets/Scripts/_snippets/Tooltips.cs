using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SaFrLib {
	public class Tooltips : MonoBehaviour {

		public static string defaultPrefabPath = "SaFrLib/Default Tooltip";

		/// TOOLTIPS
		/// ===============
		/// - WHAT
		/// 	Tooltips provide UI popups to (a) CONVEY INFORMATION to the player and
		/// 	(b) ENABLE ACTIONS that the player can take.
		/// - HOW
		/// 	Call Tooltips.CreateNew with any of the following options. Using named variables can help
		/// 	with the lack of overload functions - see the `Tooltips.CreateNew` implementation example
		/// 	at the bottom of the script for more information.
		/// 
		/// 	- string toDisplay (required) - the text to display in the Tooltip
		/// 	- RectTransform parent (default null, optional) - the RectTransform parent of this Tooltip.
		/// 		Defaults to the first Canvas in the scene and creates a Canvas if none is in place.
		/// 	- Vector3 position (default default(Vector3), optional) - the center of the spawned Tooltip
		/// 	- Transform toFollow (default null, optional) - the Transform this tooltip will follow
		/// 	- Vector3 offset (default default(Vector3), optional, requires toFollow) - the distance between this
		/// 		Tooltip and the transform it will follow
		/// 	- TooltipCallback onCreation (default null, optional) - callback to be fired when this	
		/// 		Tooltip is created (uses created tooltip as parameter)
		/// 	- TooltipCallback onDestruction (default null, optional) - callback to be fired when this
		/// 		Tooltip is destroyed (uses created tooltip as parameter)
		/// 	- string exitText (default "Continue", optional) - text to display on 'Continue...' button (defaults
		/// 		to 'Continue')
		/// 	- string[] exitChoices (default null, optional) - creates buttons that destroy the Tooltip and invoke
		/// 		the corresponding function in exitCallbacks (requires exitCallbacks to be
		/// 		defined)
		/// 	- TooltipCallback[] exitCallbacks (default null, optional) - methods to invoke when the corresponding
		/// 		exitChoice button is pressed
		/// 	- Tooltip toCreate (default null, optional) - Override instantiating the Tooltips prefab
		/// 		in favor of a specified prefab
		/// 	- TooltipStyleOptions style (default null, optional) - Style options for created Tooltip
		/// 	- float destroyAfter (default -1, optional) - Tooltip will self-destruct in this many seconds (-1 to skip)
		/// 
		/// - TODO
		/// 	- Style options: background color, font color, font family
		/// 	- Headline on tooltip
		/// 

		//

		/// <summary>
		/// The tooltip prefab.
		/// </summary>
		public GameObject prefab;
		public delegate void TooltipCallback(GameObject createdTooltip);

		/// <summary>
		/// Creates the new Tooltip.
		/// </summary>
		/// <returns>The new.</returns>
		/// <param name="toDisplay">String to display.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="position">Position in screen space.</param>
		/// <param name="toFollow">Object to stay on top of.</param>
		/// <param name="offset">Offset from followed object..</param>
		/// <param name="exitText">Exit text.</param>
		/// <param name="exitChoices">Exit choices.</param>
		/// <param name="exitCallbacks">Exit callbacks.</param>
		/// <param name="onCreation">On creation callback. Accepts the created Tooltip's GameObject as a parameter.</param>
		/// <param name="onDestruction">On destruction callback. Accepts the created Tooltip's GameObject as a parameter.</param>
		/// <param name="style">Style.</param>
		/// <param name="toCreate">Prefab to create.</param>
		/// <param name="destroyAfter">Destroy after a given number of seconds (-1 = never)</param>
		/// <param name="subsequentTooltips">Shortcut to subsequent tooltips. Useful for tutorials that don't require special placement, for instance.</param>
		public static Tooltip CreateNew(string toDisplay = "", RectTransform parent = null, Vector3 position = default(Vector3), 
			Transform toFollow = null, Vector3 offset = default(Vector3), 
			string exitText = "Continue", string[] exitChoices = null, TooltipCallback[] exitCallbacks = null, 
			TooltipCallback onCreation = null, TooltipCallback onDestruction = null,
			TooltipStyleOptions style = null, Tooltip toCreate = null, float destroyAfter = -1f,
			string[] subsequentTooltips = null) {

			// Look for Tooltips component in the scene
			Tooltips tooltipsMaster = FindObjectOfType<Tooltips>();
			if (tooltipsMaster == null) {
				// Create Tooltips master component if it doesn't exist
				GameObject g = new GameObject("Tooltip Master", typeof(Tooltips));
				tooltipsMaster = g.GetComponent<Tooltips>();
			}

			// Set prefab to instantiate
			if (tooltipsMaster.prefab == null) {
				tooltipsMaster.prefab = Resources.Load<GameObject>(defaultPrefabPath);
			}
			// Set GameObject to instantiate
			Tooltip toInstantiate = (toCreate == default(Tooltip) ? tooltipsMaster.prefab.GetComponent<Tooltip>() : toCreate);
			// Instantiate tooltip
			GameObject newTooltip = Instantiate<GameObject>(toInstantiate.gameObject) as GameObject;
			// Make sure new tooltip has Tooltip component
			Tooltip tooltip = SaFrMo.GetOrCreate<Tooltip> (newTooltip);
			RectTransform tooltipRecTransform = newTooltip.GetComponent<RectTransform>();

			// OnCreate callback
			if (onCreation != null) {
				onCreation.Invoke(newTooltip);
			}

			// Set text to display
			if (toDisplay.Length > 0) {
				tooltip.SetDisplayText (toDisplay);
			}

			// Save OnDestruction callback
			if (onDestruction != null) {
				tooltip.onDestruction = onDestruction;
			}

			// Set parent
			if (parent == null) {
				Canvas c = FindObjectsOfType<Canvas>().FirstOrDefault(x => x.renderMode != RenderMode.WorldSpace);
				if (c == default(Canvas)) {
					// Create Canvas and EventSystem if none exists yet
					c = new GameObject("Canvas (Created by Tooltips.cs)", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
					c.renderMode = RenderMode.ScreenSpaceOverlay;
					GameObject g = new GameObject("Event System (Created by Tooltips.cs)", typeof(EventSystem), typeof(StandaloneInputModule));
				}
				parent = c.GetComponent<RectTransform>();
			}
			// Make sure our parent is saved
			if (parent != null) {
				// Set parent
				newTooltip.transform.SetParent (parent, false);
			}

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
			if (exitChoices == null && exitText.Length > 0) {
				exitChoices = new string[] { exitText };
			}

			// Create exit text(s) and choice(s) if we have any
			if (exitChoices != null && exitChoices.Length > 0) {
				tooltip.CreateExitButtons (exitChoices, exitCallbacks);
			}

			// Set style
			if (style != null) {
				tooltip.style = style;
			}

			// Set autodestroy timer
			if (destroyAfter != -1f) {
				tooltip.SelfDestruct(destroyAfter);
			}

			// Set up subsequent Tooltips
			if (subsequentTooltips != null && subsequentTooltips.Length > 0) {
				List<string> allExceptFirst = new List<string>(subsequentTooltips);
				allExceptFirst.RemoveAt(0);
				string first = subsequentTooltips[0];
				tooltip.onDestruction = x => {
					Tooltips.CreateNew(first, 
						parent: parent, position: position, toFollow: toFollow, offset: offset, exitText: exitText, toCreate: toCreate, destroyAfter: destroyAfter,
						subsequentTooltips: allExceptFirst.ToArray<string>());
				};
			}

			// Return instantiated Tooltip
			return tooltip;
		}

		// Example implementation
		/*
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
		*/


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