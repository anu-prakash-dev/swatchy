using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
	public class Swatch : ScriptableObject {
		public Color[] colors;

		public Color GetColor(int colorIndex) {
			if (colors == null || colors.Length <= colorIndex || colorIndex < 0) {
				return Color.white;
			}
			return colors[colorIndex];
		}

		public static Swatch FromSwatchASEFile(SwatchASEFile file) {
			var swatchScriptableObject = ScriptableObject.CreateInstance<Swatch>();
			swatchScriptableObject.colors = new Color[file.colors.Count];
			for (int i = 0; i < swatchScriptableObject.colors.Length; i++) {
				swatchScriptableObject.colors[i] = new Color(file.colors[i].r, file.colors[i].g, file.colors[i].b);
			}
			return swatchScriptableObject;
		}

		public void AddColorsFromASEFile(SwatchASEFile file) {
			int i = this.colors.Length;
			Array.Resize<Color>(ref this.colors, this.colors.Length + file.colors.Count);
			var iterator = file.colors.GetEnumerator();
			while (iterator.MoveNext()) {
				var fileColor = iterator.Current;
				this.colors[i++] = new Color(fileColor.r, fileColor.g, fileColor.b);
			}
		}

		public void AddColorsFromOtherSwatch(Swatch otherSwatch) {
			int i = this.colors.Length;
			Array.Resize<Color>(ref this.colors, this.colors.Length + otherSwatch.colors.Length);
			for (int j = 0; j < otherSwatch.colors.Length; j++) { 
				this.colors[i++] = otherSwatch.colors[j];
			}
		}
	}
}
