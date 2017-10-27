using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartWeakEvent;
//https://www.codeproject.com/Articles/29922/Weak-Events-in-C#heading0002
//https://gist.github.com/dgrunwald/6445360
namespace Swatchy {
	public class Swatch : ScriptableObject {
		public Color[] colors;

		public event Action OnSwatchChanged2;

		public event EventHandler OnSwatchChanged {
			add { _event.Add(value); }
			remove { _event.Remove(value); }
		}

		[NonSerialized]
		private FastSmartWeakEvent<EventHandler> _event = new FastSmartWeakEvent<EventHandler>();

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
			if (this.colors == null) {
				this.colors = new Color[0];
			}
			int i = this.colors.Length;
			Array.Resize<Color>(ref this.colors, this.colors.Length + otherSwatch.colors.Length);
			for (int j = 0; j < otherSwatch.colors.Length; j++) { 
				this.colors[i++] = otherSwatch.colors[j];
			}
		}

		public void ReplaceSelfWithOtherSwatch(Swatch otherSwatch) {
			Array.Resize<Color>(ref colors, otherSwatch.colors.Length);
			Array.Copy(otherSwatch.colors, colors, otherSwatch.colors.Length);
			SignalChange();

			/*
			if (OnSwatchChanged != null)
				OnSwatchChanged();
			else
				Debug.Log("Swatch this event was null");
			*/
		}

		public void SignalChange() {
			_event.Raise(this, EventArgs.Empty);
		}
	}
}
