using System;
using UnityEngine;

namespace Swatchy {
	[Serializable]
	public class SwatchyColor {
		public SwatchyColor() {
			Debug.Log("[SwatchyColor] Constructor");
		}
		
		public Swatch swatch {
			get {return _swatch;}
			set {_swatch = value;
				if (OnColorChanged != null) OnColorChanged();
			}
		}

		public int colorIndex {
			get {return _colorIndex;}
			set {_colorIndex = value;
				if (OnColorChanged != null) OnColorChanged();
			}
		}
		

		public Color color {
			get {
				if (swatch == null || swatch.colors == null || swatch.colors.Length <= colorIndex || colorIndex < 0) {
					return Color.white;
				}
				return swatch.colors[colorIndex];
			}
		}

		[SerializeField]
		public Swatch _swatch;
		[SerializeField]
		public int _colorIndex;

		public event Action OnColorChanged;


	}
}