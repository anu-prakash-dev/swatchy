using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Swatchy {
	public class SwatchAssetImporter : AssetPostprocessor {


		/*
		  private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath) {
			for (int i = 0; i < importedAssets.Length; i++) {
				var fileName = importedAssets[i];
				if (fileName.EndsWith(".ase")) {
					Swatchy.SwatchCreator.CreateSwatchFromASEFile(fileName);
				}
			}
		}
		*/
	}
}
