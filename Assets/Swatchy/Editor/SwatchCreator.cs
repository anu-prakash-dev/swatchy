﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Swatchy {
	public static class SwatchCreator {
		[MenuItem("Assets/Swatchy/Create Swatchy Group")]
		public static void CreateSwatchyGoup() {
			SwappableSwatch asset = ScriptableObject.CreateInstance<SwappableSwatch>();
			ProjectWindowUtil.CreateAsset(asset, "New Swatchy Group.asset");
		}

		[MenuItem("Assets/Swatchy/Create Swatchy Palette")]
		public static void CreateSwatch() {
			Swatch asset = ScriptableObject.CreateInstance<Swatch>();
			ProjectWindowUtil.CreateAsset(asset, "New Swatch.asset");
		}

		public static Swatch CreateSwatchFromASEFile(SwatchASEFile aseFile, string projectSaveDestination) {
			Swatch swatch = Swatch.FromSwatchASEFile(aseFile);

			AssetDatabase.CreateAsset(swatch, projectSaveDestination);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = swatch;
			return swatch;
		}


		[MenuItem("Assets/Swatchy/Import ASE File")]
		static void ImportSelectedASEFile() {
			var activeObject = Selection.activeObject;
			var path = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());
			var fullPath = path.Replace("Assets", Application.dataPath);
			var saveDestination = path.Replace(".ase", ".asset");
			Debug.Log("[Swatchy] Import path: " + path);

			var aseFile = new SwatchASEFile(fullPath);
			CreateSwatchFromASEFile(aseFile, saveDestination);
		}

		
		[MenuItem("Assets/Swatchy/Import ASE File", true)]
		static bool ValidateImportSelectedASEFile() {
			var activeObject = Selection.activeObject;
			if (activeObject == null) {
				return false;
			}
			var path = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());
			return path.EndsWith(".ase");
		}

		[MenuItem("Assets/Swatchy/Import ASE File (Browse...)")]
		static void ImportASEFileBrowse() {
			var path = EditorUtility.OpenFilePanel("Swatchy Import", "", "ase");
			if (path != null && path != string.Empty) {
				SwatchASEFile aseFile = new SwatchASEFile(path);
				CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
			}
		}

		[MenuItem("Assets/Swatchy/Import ASE Folder Into One Pallete")]
		static void ImportASEFolderIntoOne() {
			var path = EditorUtility.OpenFolderPanel("Swatchy Import Folder", "", "");
			if (path != null && path != string.Empty) {
				var files = Directory.GetFiles(path);
				Swatch parentSwatch = null;
				for (int i = 0; i < files.Length; i++) {
					string file = files[i];
					if (file.EndsWith(".ase")) {
						SwatchASEFile aseFile = new SwatchASEFile(file);
						if (parentSwatch == null) {
							parentSwatch = CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
						}
						else {
							parentSwatch.AddColorsFromASEFile(aseFile);
						}
					}
				}
			}
		}

		[MenuItem("Assets/Swatchy/Import ASE Folder Into Many Palletes")]
		static void ImportASEFolderIntoMany() {
			var path = EditorUtility.OpenFolderPanel("Swatchy Import Folder", "", "");
			if (path != null && path != string.Empty) {
				var files = Directory.GetFiles(path);
				for (int i = 0; i < files.Length; i++) {
					string file = files[i];
					if (file.EndsWith(".ase")) {
						SwatchASEFile aseFile = new SwatchASEFile(file);
						CreateSwatchFromASEFile(aseFile, GetSelectedSavePath(aseFile.title));
					}
				}
			}
		}

		static string GetSelectedSavePath(string title) {
			return AssetDatabase.GenerateUniqueAssetPath(GetSelectedPath() + "/" + title + ".asset");
		}

		static string GetSelectedPath() {
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "") {
				path = "Assets";
			}
			else if (Path.GetExtension(path) != "") {
				path = path.Replace(Path.GetFileName(path), "");
			}
			return path;
		}
	}
}
