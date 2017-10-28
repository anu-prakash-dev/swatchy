using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
	[RequireComponent(typeof(Camera))]
	public class SwatchyCameraColor : SwatchyColorApplier {
		[HideInInspector]
		public Camera swatchingCamera;
		public override void Apply() {
			if (swatchingCamera == null) {
				swatchingCamera = GetComponent<Camera>();
			}
			swatchingCamera.backgroundColor = swatchyColor.color;
		}
	}
}
