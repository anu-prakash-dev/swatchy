using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SwatchyRenderer
//  Applies a SwatchyColor in OnEnable to the connected Renderer's material.
//  Does this by setting "_Color" on the renderer's Material Property Block
namespace Swatchy {
	[RequireComponent(typeof(Renderer))]
	public class SwatchyRenderer : SwatchyColorApplier {

		[HideInInspector]
		public Renderer renderer;

		static MaterialPropertyBlock mpb;
		static int colorId;

		public override void Apply() {
			if (mpb == null) {
				mpb = new MaterialPropertyBlock();
				colorId = Shader.PropertyToID("_Color");
			}
			if (renderer == null) {
				renderer = GetComponent<Renderer>();
			}
			mpb.SetColor(colorId, swatchyColor.color);
			renderer.SetPropertyBlock(mpb);
		}
	}
}