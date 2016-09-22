using UnityEngine;
using System.Collections;

namespace SaFrLib {
	public class TextDisplay : MonoBehaviour {

		/// TEXT DISPLAY
		/// ==================
		/// 
		/// WHAT:
		/// 	A self-contained and simple way to display text. Intended for
		/// 	Zelda-style dialogue (https://youtu.be/vjIaokM--Pk?t=23s)
		/// 
		/// HOW:
		/// 	Call TextDisplay.Display(string toDisplay) or TextDisplay.Display(string[] toDisplay)
		/// 	to start up the display. You can edit the prefab used in Resources/SaFrLib/TextDisplay.
		/// 
		/// 	Additionally, you can add a TextDisplay component to any GameObject in the level and assign a custom
		/// 	TextDisplayCanvas to display text.
		/// 

		//

		[Tooltip("Leave blank to use default prefab.")]
		public TextDisplayCanvas customCanvasPrefab;

		/// <summary>
		/// The default prefab path, relative to "Resources/".
		/// </summary>
		public static string defaultPrefabPath = "SaFrLib/Text Display Canvas";

		/// <summary>
		/// Display the specified string. Convenience wrapper for Display(string[] toDisplay)
		/// </summary>
		/// <param name="toDisplay">To display.</param>
		public static void Display(string toDisplay) {
			Display(new string[] { toDisplay} );
		}

		/// <summary>
		/// Display the specified strings, one after the other.
		/// </summary>
		/// <param name="toDisplay">To display.</param>
		public static void Display(string[] toDisplay) {
			
			// Instantiate prefab if it doesn't exist yet, OR find it if it does
			TextDisplayCanvas canvas = FindObjectOfType<TextDisplayCanvas>();
			if (canvas == null) {
				
				GameObject instantiatedCanvas;

				// Look for existing TextDisplay
				TextDisplay textDisplayInstance = FindObjectOfType<TextDisplay>();
				if (textDisplayInstance != null && textDisplayInstance.customCanvasPrefab != null) {
					// We've created a TextDisplay and we've set a custom prefab
					instantiatedCanvas = Instantiate(textDisplayInstance.customCanvasPrefab.gameObject) as GameObject;
				} else {
					// We're using the default prefab (either by not creating a TextDisplay component or leaving the canvasPrefab field blank)
					instantiatedCanvas = Instantiate(Resources.Load<GameObject>(defaultPrefabPath)) as GameObject;
				}

				// Find TextDisplayCanvas attached to instantiated canvas and save it
				canvas = instantiatedCanvas.GetComponent<TextDisplayCanvas>();

				// Log error if no attached TextDisplayCanvas
				if (canvas == null) {
					Debug.LogError("No TextDisplayCanvas found attached to GameObject " + instantiatedCanvas.name + "! " +
						"Did you attach a TextDisplayCanvas to your custom text display, or alter the contents of Resources/SaFrLib?");
				}
				
			}

			// Populate prefab with string[] content and run the prefab's display function
			canvas.SetContent(toDisplay);
		}
	}
}