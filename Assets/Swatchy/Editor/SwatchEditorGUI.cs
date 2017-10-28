using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Swatchy {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Swatch))]
	public class SwatchEditorGUI : Editor {
		private bool merge;
		private bool replace;
		private Swatch mergeObject;
		private Swatch replaceObject;

		public override void OnInspectorGUI() {
			Swatch swatch = (Swatch)target;
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck()) {
				swatch.SignalChange();
				SwatchEditorGUI.GameViewRepaint();
			}

			if (GUILayout.Button("Add .ASE")) {
				var path = EditorUtility.OpenFilePanel("Swatchy Import", "", "ase");
				if (path != null && path != string.Empty) {
					Debug.Log("[SwatchEditorGUI] path " + path);
					SwatchASEFile aseFile = new SwatchASEFile(path);
					swatch.AddColorsFromASEFile(aseFile);
				}
			}

			if (GUILayout.Button("Add .ASE Folder")) {
				var path = EditorUtility.OpenFolderPanel("Swatchy Folder Import", "", "");
				//var path = EditorUtility.OpenFilePanel("Import", "", "ase");
				if (path != null && path != string.Empty) {
					var files = Directory.GetFiles(path);
					for (int i = 0; i < files.Length; i++) {
						string file = files[i];
						if (file.EndsWith(".ase")) {
							SwatchASEFile aseFile = new SwatchASEFile(file);
							swatch.AddColorsFromASEFile(aseFile);
						}
					}
				}
			}

			if (GUILayout.Button(replace ? "Cancel Replace" : "Replace With Another Swatch")) {
				replace = !replace;
				if (replace) {
					LoadOtherSwatches();
				}
			}

			if (replace) {
				if (otherSwatchGUIDs != null && otherSwatchFilenames != null) {
					var lastRect = GUILayoutUtility.GetLastRect();
					var indent = 50;
					lastRect.x += indent;
					lastRect.width -= indent;
					var spacing = 3;
					GUILayout.Space((EditorGUIUtility.singleLineHeight+spacing) * otherSwatchGUIDs.Length);
					for (int i = 0; i < otherSwatchGUIDs.Length; i++) {
						var swatchName = otherSwatchFilenames[i];

						var buttonRect = new Rect(lastRect.x, lastRect.y + EditorGUIUtility.singleLineHeight + spacing, lastRect.width, EditorGUIUtility.singleLineHeight);
						lastRect = buttonRect;
						if (GUI.Button(buttonRect, swatchName)) {
							var otherSwatchAssetPath = AssetDatabase.GUIDToAssetPath(otherSwatchGUIDs[i]);
							var otherSwatch = AssetDatabase.LoadAssetAtPath<Swatch>(otherSwatchAssetPath);
							if (otherSwatch != null) {
								swatch.ReplaceSelfWithOtherSwatch(otherSwatch);
								SwatchEditorGUI.GameViewRepaint();
							}
							else {
								Debug.LogError("[SwatchEditorGUI] couldnt load asset at path: " +otherSwatchAssetPath);
							}
							break;
						}
					}
				}
			}


			if (GUILayout.Button(merge ? "Cancel Merge" : "Merge With Another Swatch")) {
				mergeObject = null;
				merge = !merge;
			}
			if (merge) {
				mergeObject = (Swatch)EditorGUILayout.ObjectField(mergeObject, typeof(Swatch), false);
				if (mergeObject != null) {
					if (GUILayout.Button("Merge")) {
						swatch.AddColorsFromOtherSwatch(mergeObject);
						mergeObject = null;
						merge = false;
					}

				}
			}




			if (GUILayout.Button("Export To Color Picker Presets")) {
				SwatchPresetExporter.ExportToColorPresetLibrary(swatch);
			}
		}

		string[] otherSwatchGUIDs;
		string[] otherSwatchFilenames;

		void LoadOtherSwatches() {
			var swatchGUIDs = AssetDatabase.FindAssets("t:Swatch");
			var selfGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target.GetInstanceID()));
			int iterator = 0;
			int numOtherSwatches = swatchGUIDs.Length - 1;
			otherSwatchGUIDs = new string[numOtherSwatches];
			otherSwatchFilenames = new string[numOtherSwatches];
			for (int i = 0; i < swatchGUIDs.Length; i++) {
				if (swatchGUIDs[i].Equals(selfGUID)) {
					continue;
				}
				var swatchPath = AssetDatabase.GUIDToAssetPath(swatchGUIDs[i]);
				var swatchName = System.IO.Path.GetFileNameWithoutExtension(swatchPath);

				otherSwatchGUIDs[iterator] = swatchGUIDs[i];
				otherSwatchFilenames[iterator++] = swatchName;
			}
		}

		static EditorWindow gameview;
		public static void GameViewRepaint() {
			if (gameview == null) {
				System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
				System.Type type = assembly.GetType("UnityEditor.GameView");
				gameview = EditorWindow.GetWindow(type);
			}
			if (gameview != null) {
				gameview.Repaint();
			}
		}
	}
}