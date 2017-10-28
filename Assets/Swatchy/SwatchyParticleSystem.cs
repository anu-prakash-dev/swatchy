using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swatchy {
	[RequireComponent(typeof(ParticleSystem))]
	public class SwatchyParticleSystem : SwatchyColorApplier {
		ParticleSystem swatchingParticleSystem;
		public override void Apply() {
			if (swatchingParticleSystem == null) {
				swatchingParticleSystem = GetComponent<ParticleSystem>();
			}
			var main = swatchingParticleSystem.main;
			main.startColor = swatchyColor.color;
		}
	}
}
