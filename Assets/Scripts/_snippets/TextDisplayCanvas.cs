using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SaFrLib {
	public class TextDisplayCanvas : MonoBehaviour {

		protected string[] content;

		public virtual void SetContent(string[] newContent, bool displayImmediately = true) {
			content = newContent;

			if (displayImmediately) {
				DisplayContent();
			}
		}

		public virtual void DisplayContent() {

		}
	}
}