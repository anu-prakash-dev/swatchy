using System;
using UnityEngine;

namespace Swatchy {
	[Serializable]
	public class SwatchyColor {
		public Swatch swatch;
		public int colorIndex;

		public Color color {
			get { 
				if (swatch == null || swatch.colors == null || swatch.colors.Length <= colorIndex || colorIndex < 0) {
					return Color.white;
				}
				return swatch.colors[colorIndex];
			}
		}
	}
}