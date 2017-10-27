using System;
using UnityEngine;

namespace Swatchy {
	// SwatchyColor
	//  Holds a Swatch and an index into that Swatch.
	//  Returns a color using the color index and the swatch.
	//  Implements the "Observer Pattern" allowing classes to listen
	//  to changes in the swatch or colorIndex.
	//  Calls OnColorChanged when the swatch or the colorIndex has changed.
	//  In order to react to changes within the swatch, we need to 
	//  subscribe to OnSwatchChanged events.
	//  We use a "weak delegate" pattern so that we never have to
	//  un register for the event. This is useful because this class
	//  cannot simply deregister in the destructor because the destructor
	//  won't get called if we use a regular event subscription pattern.
	//  More info on weak delegates can be found here:
	//  https://www.codeproject.com/Articles/29922/Weak-Events-in-C#heading0002
	[Serializable]
	public class SwatchyColor {
		public SwatchyColor() {
			Debug.Log("[SwatchyColor] Constructor");
		}
		void OnSwatchChanged(object sender, EventArgs e) {
			Debug.Log("[SwatchyColor] OnSwatchChanged");
			if (OnColorChanged != null)
				OnColorChanged();
		}

		/*
		~SwatchyColor() {
			swatch.OnSwatchChanged -= OnSwatchChanged;
			Debug.Log("[SwatchyColor] Destructor");
			//swatch.OnSwatchChanged -= _onSwatchChangedStrongReference;
		}
		*/


		public Swatch swatch {
			get {return _swatch;}
			set {
				if (_swatch != null)
					_swatch.OnSwatchChanged -= OnSwatchChanged;
				_swatch = value;
				if (_swatch != null)
					_swatch.OnSwatchChanged += OnSwatchChanged;
				Debug.Log("[SwatchyColor] Subscribed to changes");
				if (OnColorChanged != null)
					OnColorChanged();
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