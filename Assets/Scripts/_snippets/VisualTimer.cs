using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

/// VISUAL TIMER
/// A portable progress bar for your games!
/// 
/// WHAT
/// 	The Visual Timer shows the time until an event is complete. It can count up or down,
/// 	make use of linear or radial fills, use custom prefabs, pause, and more.
/// 
/// HOW
/// 	Simple usage:
/// 		1. Call VisualTimer.CreateTimer and pass the appropriate parameters.
/// 		2. CreateTimer returns the new VisualTimer, which you can manipulate as needed.

namespace SaFrLib {
	public class VisualTimer : MonoBehaviour {

		/// <summary>
		/// The total time.
		/// </summary>
		public float totalTime { get; protected set; }
		/// <summary>
		/// The time remaining.
		/// </summary>
		private float timeRemaining = 0;

		/// <summary>
		/// Whether or not this timer self destructs on complete.
		/// </summary>
		public bool selfDestructOnComplete = true;
		/// <summary>
		/// Whether or not this timer uses the in-game time scale.
		/// </summary>
		public bool useTimeScale = true;
		/// <summary>
		/// Whether or not this timer is paused. Paused timers do not register any time changes.
		/// </summary>
		public bool paused = false;

		/// <summary>
		/// The timer sprite. This is the sprite that will change depending on total time and time remaining.
		/// </summary>
		public Image timerSprite;
		/// <summary>
		/// The timer text. This will display time remaining, rounded to the nearest hundredth of a second.
		/// </summary>
		public Text timerText;

		// Callbacks
		public UnityEvent onSpawn, onTimerSet, onTimerComplete;

		protected virtual void Start() {

		}

		/// <summary>
		/// Sets the timer and starts it running.
		/// </summary>
		/// <param name="time">Time.</param>
		public virtual void SetTimer(float time) {
			totalTime = time;
			timeRemaining = time;

			// Invoke callback
			if (onTimerSet != null)
				onTimerSet.Invoke ();
		}

		/// <summary>
		/// Jumps to a fraction of time remaining, regardless of current time remaining.
		/// For example, a 2-second timer receiving JumpToFraction(0.5f) would jump to
		/// 1 second remaining.
		/// 
		/// If the fraction is above 1 and hardReset is true, the timer resets itself entirely to the new value.
		/// Otherwise, it clamps to the old timer value.
		/// </summary>
		/// <param name="fractionLeft">Fraction left.</param>
		public virtual void JumpToFraction(float fractionLeft, bool hardReset = true) {
			if (fractionLeft >= 1f && hardReset) {
				SetTimer (totalTime * fractionLeft);
			} else {
				timeRemaining = Mathf.Min (totalTime, totalTime * fractionLeft);
			}

			// Invoke callback
			if (onTimerSet != null)
				onTimerSet.Invoke ();
		}

		/// <summary>
		/// Jumps to time remaining. If hardReset is true, the timer will reset itself to timeLeft if
		/// timeLeft >= totalTime. Otherwise, the remaining time will be set to the lowest value of timeLeft and totalTime.
		/// 
		/// For example, using a 2-second timer:
		/// * JumpToTimeRemaining(1f) jumps to 1 second left.
		/// * JumpToTimeRemaining(3f) jumps to 3 seconds left, since hardReset is true.
		/// * JumpToTimeRemaining(3f, false) jumps to 2 seconds left, since hardReset is false.
		/// </summary>
		/// <param name="timeLeft">Time left.</param>
		/// <param name="hardReset">If set to <c>true</c> hard reset.</param>
		public virtual void JumpToTimeRemaining(float timeLeft, bool hardReset = true) {
			if (timeLeft >= totalTime && hardReset) {
				SetTimer (timeLeft);
			} else {
				timeRemaining = Mathf.Min (totalTime, timeLeft);
			}

			// Invoke callback
			if (onTimerSet != null)
				onTimerSet.Invoke ();
		}

		/// <summary>
		/// Pauses or unpauses this timer.
		/// </summary>
		/// <param name="toPause">If set to <c>true</c> to pause.</param>
		public virtual void SetPause(bool toPause = true) {
			paused = toPause;
		}

		public virtual void SetFollowTarget(Transform target) {

		}

		protected virtual void Update() {

			// Return if paused
			if (paused) {
				return;
			}

			// Run down time remaining
			timeRemaining -= useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;

			// Update image
			if (timerSprite != null) {
				timerSprite.fillAmount = timeRemaining / totalTime;
			}
			// Update text
			if (timerText != null) {
				timerText.text = timeRemaining.ToString ("F2");
			}

			// Are we <= 0 time remaining?
			if (timeRemaining <= 0) {
				// If so, run onTimerComplete
				if (onTimerComplete != null)
					onTimerComplete.Invoke ();
				// Self-destruct if desired
				if (selfDestructOnComplete) {
					Destroy (gameObject);
				}
			}

		}



		// STATIC INSTANTIATION

		private const string defaultPath = "SaFrLib/Default Visual Timer";

		/// <summary>
		/// Creates an instance of the timer.
		/// </summary>
		/// <returns>The timer.</returns>
		public static VisualTimer CreateTimer(float time, Transform parent = null, Vector3 position = default(Vector3), Transform toFollow = null, string prefabPath = "", VisualTimer prefab = null) {

			// We don't have a prefab specified, so load the one we want to use
			if (prefab == null) {
				// Load the specified path or the default prefab path
				if (prefabPath.Length > 0) {
					prefab = Resources.Load<VisualTimer> (prefabPath) as VisualTimer;
				} else {
					prefab = Resources.Load<VisualTimer> (defaultPath) as VisualTimer;
				}
			}
				
			if (prefab == null) {
				// There's still no prefab, so exit early and log error
				Debug.Log ("No VisualTimer prefab could be found! Make sure you're using a path that can be parsed by Resources.Load or passing an existing VisualTimer component.");
				return default(VisualTimer);
			}

			// Instantiate the desired prefab
			GameObject createdObject = Instantiate(prefab.gameObject) as GameObject;
			// Save reference to the created VisualTimer
			VisualTimer createdTimer = createdObject.GetComponent<VisualTimer> ();
			// Save reference to RectTransform
			RectTransform rectTransform = createdObject.GetComponent<RectTransform>();

			// Apply its initial timerLength
			createdTimer.SetTimer(time);

			// Find parent
			if (parent == null) {
				parent = FindObjectOfType<Canvas> ().transform;
			}

			// If the parent doesn't have a Canvas, create one
			if (parent.GetComponent<Canvas> () == null) {
				GameObject newCanvas = new GameObject ("VisualTimer Canvas (Created by VisualTimer.CreateTimer)", new System.Type[] { typeof(Canvas) });
				newCanvas.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
				newCanvas.transform.SetParent (parent, false);
				// Attach this timer to the new Canvas
				rectTransform.SetParent (newCanvas.transform);
			} else {
				// Attach to parent
				rectTransform.SetParent(parent);
			}
			// Move to position
			rectTransform.localPosition = position;

			// Set transform to follow, if specified
			if (toFollow != null) {
				createdTimer.SetFollowTarget (toFollow);
			}

			return createdTimer;
		}
	}
}