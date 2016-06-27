using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SaFrLib {
	public class Tooltip : MonoBehaviour {

		public Transform toFollow;
		public Vector3 toFollowOffset;
		public MenuRefresher exitButtons;

		public Tooltips.TooltipCallback onDestruction;

		void OnDestroy() {
			if (onDestruction != null) {
				onDestruction.Invoke(gameObject);
			}
		}

	}
}