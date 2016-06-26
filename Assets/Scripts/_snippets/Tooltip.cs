using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SaFrLib {
	public class Tooltip : MonoBehaviour {

		public Tooltips.TooltipCallback onDestruction;

		void OnDestroy() {
			if (onDestruction != null) {
				onDestruction.Invoke(gameObject);
			}
		}

	}
}