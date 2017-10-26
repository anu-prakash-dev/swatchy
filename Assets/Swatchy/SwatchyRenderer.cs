using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SwatchyRenderer
//  Applies a SwatchyColor in Awake to the connected Renderer's material.
//  Does this by setting "_Color" on the renderer's Material Property Block
namespace Swatchy {
	[RequireComponent(typeof(Renderer))]
	public class SwatchyRenderer : MonoBehaviour {

		public SwatchyColor swatchyColor;
		[HideInInspector]
		public Renderer renderer;

		static MaterialPropertyBlock mpb;
		static int colorId;

		void Awake() {
			Apply();
			swatchyColor.OnColorChanged += Apply;
		}
		void OnDestroy() {
			swatchyColor.OnColorChanged -= Apply;
		}

		// a hack to subscribe to this object after it's created in the editor
#if UNITY_EDITOR
		private bool hasSubscribed;
		void OnValidate() {
			Debug.Log("[OnValidate] is not null" + (swatchyColor != null).ToString());
			if (swatchyColor != null && !hasSubscribed) {
				swatchyColor.OnColorChanged += Apply;
				hasSubscribed = true;
			}
		}
#endif

		public void Apply() {
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