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
		private Swatch mergeObject;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			Swatch swatch = (Swatch)target;
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




			if (GUILayout.Button(merge ? "Cancel Merge" : "Merge With Another Swatch")) {
				mergeObject = null;
				merge = !merge;
			}
			if (merge) {
				mergeObject = (Swatch)EditorGUILayout.ObjectField(mergeObject, typeof(Swatch));
				if (mergeObject != null) {
					if (GUILayout.Button("Merge")) {
						swatch.AddColorsFromOtherSwatch(mergeObject);
						mergeObject = null;
						merge = false;
					}

				}
			}


			if (GUILayout.Button("Export To Library")) {
				SwatchPresetExporter.ExportToColorPresetLibrary(swatch);
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