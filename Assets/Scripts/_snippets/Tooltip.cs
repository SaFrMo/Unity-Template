using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace SaFrLib {
	public class Tooltip : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

		// TOOLTIP
		// Attach this as a component to the GameObject you want to instantiate as a Tooltip.

		public Transform toFollow;
		public Vector3 toFollowOffset = Vector3.zero;
		public Vector3 viewportPadding { 
			get { 
				if (style == null || style.viewportPadding == default(Vector3)) {
					return Vector3.zero;
				} else {
					return style.viewportPadding; 
				}
			} 
		}
		public MenuRefresher exitButtons;
		public Text body;
		public bool canGoOffscreen = false;
		public TooltipStyleOptions style;

		// Rect convenience variables
		RectTransform rect;
		public float width { get { return rect.rect.width; } }
		public float height { get { return rect.rect.height; } }
		public float halfWidth { get { return width / 2f; } }
		public float halfHeight { get { return height / 2f; } }

		public Tooltips.TooltipCallback onDestruction;

		// Dragging variables
		Vector3 dragOffset;

		void Start() {
			rect = GetComponent<RectTransform>();

			if (style == null) {
				style = new TooltipStyleOptions();
			}
		}

		#region IBeginDragHandler implementation

		public virtual void OnBeginDrag (PointerEventData eventData)
		{
			dragOffset = rect.position - new Vector3(eventData.position.x, eventData.position.y);
		}

		#endregion

		#region IDragHandler implementation

		public virtual void OnDrag (PointerEventData eventData)
		{
			if (style == null || style.draggable) {
				rect.position = new Vector3(eventData.position.x, eventData.position.y) + dragOffset;
			}
		}

		#endregion

		#region IEndDragHandler implementation

		public virtual void OnEndDrag (PointerEventData eventData)
		{
		}

		#endregion

		public void CreateExitButtons(string[] toDisplay, Tooltips.TooltipCallback[] callbacks) {
			// Exit if we don't have exit buttons defined
			if (exitButtons == null) {
				return;
			}

			int i = 0;
			exitButtons.Setup<string>(toDisplay, (createdButton, text) => {
				createdButton.GetComponentInChildren<Text>().text = text;

				Tooltips.TooltipCallback callback;
				if (callbacks == null || callbacks.Length <= i)
					callback = null;
				else
					callback = callbacks[i];

				SetupButton(createdButton.GetComponent<Button>(), callback);
				i++;
			});
		}

		void SetupButton(Button button, Tooltips.TooltipCallback callback) {
			button.onClick.AddListener(() => {
				if (callback != null)
					callback.Invoke(gameObject);
				ExitTooltip();
			});
		}

		public void SelfDestruct(float delay) {
			Invoke("DestroyMe", delay);
		}

		public void DestroyMe() {
			if (gameObject) {
				Destroy(gameObject);
			}
		}

		public void SetDisplayText(string text) {
			body.text = text;
		}

		public void ExitTooltip() {
			Destroy(gameObject);
		}

		void OnDestroy() {
			if (onDestruction != null) {
				onDestruction.Invoke(gameObject);
			}
		}

		void MoveTo(Vector3 screenPoint) {
			Vector3 targetPos = screenPoint;
			if (!canGoOffscreen) {
				float minX = halfWidth + viewportPadding.x;
				float maxX = Screen.width - halfWidth - viewportPadding.x;
				float minY = halfHeight + viewportPadding.y;
				float maxY = Screen.height - halfHeight - viewportPadding.y;
				targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
				targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
			}
			transform.position = targetPos;
		}

		protected virtual void Update() {
			if (toFollow != null) {
				MoveTo(Camera.main.WorldToScreenPoint(toFollow.position) + toFollowOffset);
			}
		}

	}
}