using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
[RequireComponent(typeof(Light))]
public class SwatchyLight : SwatchyColorApplier {
	private Light light;
	public override void Apply () {
		if (light == null) {
			light = GetComponent<Light>();
		}
		light.color = swatchyColor.color;
	}
}
}
