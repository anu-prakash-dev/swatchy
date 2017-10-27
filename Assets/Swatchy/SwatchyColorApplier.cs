using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
	[ExecuteInEditMode]
	public abstract class SwatchyColorApplier : MonoBehaviour {
		public SwatchyColor swatchyColor;

		void OnDestroy() {
			swatchyColor.OnColorChanged -= Apply;
		}

		void OnDisable() {
			swatchyColor.OnColorChanged -= Apply;
		}

		void OnEnable() {
			if (swatchyColor == null) {
				swatchyColor = new SwatchyColor();
			}
			swatchyColor.OnColorChanged += Apply;
			swatchyColor.OnEnable();
		}

		public abstract void Apply();
	}
}
