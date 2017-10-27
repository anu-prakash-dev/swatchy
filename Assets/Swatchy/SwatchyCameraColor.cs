using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
	[RequireComponent(typeof(Camera))]
	public class SwatchyCameraColor : SwatchyColorApplier {
		[HideInInspector]
		public Camera camera;
		public override void Apply() {
			if (camera == null) {
				camera = GetComponent<Camera>();
			}
			camera.backgroundColor = swatchyColor.color;
		}
	}
}
