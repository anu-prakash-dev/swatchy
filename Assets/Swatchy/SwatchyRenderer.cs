using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SwatchyRenderer
//  Applies a SwatchyColor in Awake to the connected Renderer's material.
//  Does this by setting "_Color" on the renderer's Material Property Block
namespace Swatchy {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class SwatchyRenderer : MonoBehaviour {

		public SwatchyColor swatchyColor;
		[HideInInspector]
		public Renderer renderer;

		static MaterialPropertyBlock mpb;
		static int colorId;

		void Awake() {
			if (swatchyColor == null) {
				swatchyColor = new SwatchyColor();
			}
			Apply();
			swatchyColor.OnColorChanged += Apply;
			hasSubscribed = true;
			Debug.Log("[Awake] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());

		}
		/*
		void OnDestroy() {
			swatchyColor.OnColorChanged -= Apply;
		}
		*/


		// a hack to subscribe to this object after it's created in the editor
#if UNITY_EDITOR
		private bool hasSubscribed;
		void OnValidate() {
			Debug.Log("[OnValidate] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());
			if (swatchyColor != null && !hasSubscribed) {
				swatchyColor.OnColorChanged += Apply;
				hasSubscribed = true;
				Apply();
			}
		}


		void OnDestroy() {
			Debug.Log("[OnDestroy] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());
		}
		void OnDisable() {
			Debug.Log("[OnDisable] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());
			hasSubscribed = false;
		}
		void OnEnable() {
			Debug.Log("[OnEnable] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());
		}
		void Reset() {
			Debug.Log("[Reset] hasSubscribed ; " +hasSubscribed+" is not null " + (swatchyColor != null).ToString());
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