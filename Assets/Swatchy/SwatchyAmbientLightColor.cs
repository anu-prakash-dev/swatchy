using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swatchy;
namespace Swatchy {
public class SwatchyAmbientLightColor : SwatchyColorApplier {


	public override void Apply () {
		if (RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Flat) {
			Debug.LogWarning("[SwatchyAmbientLightColor] Ambient light mode is not set to flat. Changing it now. Change it manually in Lighting->Scene.");
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		}
		RenderSettings.ambientLight = swatchyColor.color;
	}
}
}