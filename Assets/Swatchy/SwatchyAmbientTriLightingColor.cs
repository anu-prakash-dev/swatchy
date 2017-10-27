using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
[ExecuteInEditMode]
public class SwatchyAmbientTriLightingColor : MonoBehaviour {
		[Header("Warning: This component changes scene settings in Lighting->Scene")]
		public SwatchyColor sky;
		public SwatchyColor equator;
		public SwatchyColor ground;

		void OnDestroy() {
			sky.OnColorChanged -= Apply;
			equator.OnColorChanged -= Apply;
			ground.OnColorChanged -= Apply;
		}

		void OnDisable() {
			sky.OnColorChanged -= Apply;
			equator.OnColorChanged -= Apply;
			ground.OnColorChanged -= Apply;
		}

		void OnEnable() {
			if (sky == null) {
				sky = new SwatchyColor();
			}
			if (equator == null) {
				equator = new SwatchyColor();
			}
			if (ground == null) {
				ground = new SwatchyColor();
			}
			sky.OnColorChanged += Apply;
			sky.OnEnable();

			equator.OnColorChanged += Apply;
			equator.OnEnable();

			ground.OnColorChanged += Apply;
			ground.OnEnable();
		}

		public void Apply() {
			if (RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Trilight) {
				Debug.LogWarning("[SwatchyAmbientTryLightingColor] RenderSettings.ambientMode != Trilight. Changing the setting to Tri Lighting. Change it manually in Lighting->Scene.");
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
			}
			RenderSettings.ambientSkyColor 		= sky.color;
			RenderSettings.ambientEquatorColor 	= equator.color;
			RenderSettings.ambientGroundColor 	= ground.color;
		}
}
}
