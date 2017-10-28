using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
[RequireComponent(typeof(Light))]
public class SwatchyLight : SwatchyColorApplier {
	private Light swatchingLight;
	public override void Apply () {
		if (swatchingLight == null) {
			swatchingLight = GetComponent<Light>();
		}
		swatchingLight.color = swatchyColor.color;
	}
}
}
