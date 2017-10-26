using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Swatchy {
	[CustomPropertyDrawer(typeof(SwatchyColor))]
	public class SwatchyColorDrawer : PropertyDrawer {

		private Texture2D swatchTexture;
		private Texture2D palleteTexture;
		private Texture2D blackTexture;

		private bool paletteOpen;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			SwatchyColor swatchyColor = (SwatchyColor)fieldInfo.GetValue(property.serializedObject.targetObject);
			Color color = swatchyColor.color;
			var swatch = swatchyColor.swatch;
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			// var swatchColorRect = new Rect(position.x, position.y, 30, position.height);
			var swatchSize = EditorGUIUtility.singleLineHeight;
			var keySize = EditorGUIUtility.singleLineHeight*1.25f;
			var spacing = EditorGUIUtility.singleLineHeight * 0.5f;
			var swatchObjectRect = new Rect(position.x, position.y, 120, EditorGUIUtility.singleLineHeight);
			swatchObjectRect.width = swatch == null ? position.width : position.width - swatchSize - keySize - spacing * 2;
			swatchObjectRect.position = new Vector2(swatch == null ? position.x : position.x + swatchSize + keySize + spacing * 2, position.y);

			var swatchRect = new Rect(position.x, position.y, swatchSize, EditorGUIUtility.singleLineHeight);
			var colorIndexRect = new Rect(swatchRect.position.x + swatchRect.width + spacing, position.y, keySize, EditorGUIUtility.singleLineHeight);


			if (swatchTexture == null) {
				swatchTexture = textureWithColor(color);
			}
			// Draw fields - passs GUIContent.none to each so they are drawn without labels

			// Draw Swatch object
			EditorGUI.PropertyField(swatchObjectRect, property.FindPropertyRelative("_swatch"), GUIContent.none);
			// Draw Color index text field
			if (swatch != null) {
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(colorIndexRect, property.FindPropertyRelative("_colorIndex"), GUIContent.none);
				if (EditorGUI.EndChangeCheck()) {
					if (swatch != null) {
						var colorIndex = property.FindPropertyRelative("_colorIndex").intValue;
						var newcolor = swatch.GetColor(colorIndex);
						swatchyColor.colorIndex = colorIndex; // hack which calls observer pattern
						UpdateActiveSwatch(newcolor);
					}
				}



				/*
				var foldOutStyle = EditorStyles.foldout;
				foldOut = EditorGUI.Foldout(swatchRect, foldOut,"" , foldOutStyle);
				*/
				// open color window

				// Draw color square


				//		var buttonStyle = new GUIStyle(EditorStyles.label);
				var buttonStyle = new GUIStyle(GUIStyle.none);

				var style = new GUIStyle();
				style.normal.background = swatchTexture;
				//		buttonStyle.fixedHeight = 15;
				//		buttonStyle.fixedWidth = 15;
				//		buttonStyle.stretchHeight = true;
				//		buttonStyle.stretchWidth = true;

				if (GUI.Button(swatchRect, "", buttonStyle)) {
					paletteOpen = !paletteOpen && swatch != null && swatch.colors != null && swatch.colors.Length > 0;
				}

				EditorGUI.LabelField(swatchRect, "", style);
				DrawBlackGrid(swatchRect.x, swatchRect.y, 1, 1, (int)EditorGUIUtility.singleLineHeight);

				if (paletteOpen) {
					if (palleteTexture == null) {
						palleteTexture = textureWithColors(swatch.colors);
					}
					float _scale = 15;
					var textureRect = new Rect(swatchRect.x, swatchRect.y + EditorGUIUtility.singleLineHeight + 3, palleteTexture.width * EditorGUIUtility.singleLineHeight, palleteTexture.height * EditorGUIUtility.singleLineHeight);
					var palleteTextureStyle = new GUIStyle();
					palleteTextureStyle.normal.background = palleteTexture;
					//			palleteTextureStyle.border = new RectOffset(1,1,1,1);
					EditorGUI.LabelField(textureRect, "", palleteTextureStyle);
					DrawBlackGrid(textureRect.x, textureRect.y, palleteTexture.width, palleteTexture.height, (int)EditorGUIUtility.singleLineHeight);



					// listen to click
					Event e = Event.current;
					if (e != null) {
						if (e.isMouse && e.button == 0) {
							if (textureRect.Contains(e.mousePosition)) {
								Vector2 rectClickPosition = e.mousePosition - textureRect.position;
								int cellXIndex = (int)(rectClickPosition.x / EditorGUIUtility.singleLineHeight);
								int cellYIndex = (int)(rectClickPosition.y / EditorGUIUtility.singleLineHeight);
								int colorIndex = cellYIndex * palleteTexture.width + cellXIndex;
								swatchyColor.colorIndex = colorIndex;
								UpdateActiveSwatch(swatchyColor.color);
							}
							else {
								paletteOpen = false;
								EditorUtility.SetDirty(property.serializedObject.targetObject); // Repaint

							}
						}
					}
				}
			}
			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();

		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float originalHeight = base.GetPropertyHeight(property, label);
			if (!paletteOpen || palleteTexture == null)
				return originalHeight;
			else {
				return originalHeight + palleteTexture.height * EditorGUIUtility.singleLineHeight + 5;
			}
		}

		Texture2D textureWithColor(Color color) {
			var tex = new Texture2D(1, 1, TextureFormat.RGB24, false, true);
			tex.filterMode = FilterMode.Point;
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.SetPixel(0, 0, color);
			tex.Apply();
			return tex;
		}

		Texture2D textureWithColors(Color[] colors) {
			int itemsPerRow = 5;
			// figure out our texture size based on the itemsPerRow and color count
			int totalRows = Mathf.CeilToInt((float)colors.Length / (float)itemsPerRow);
			var tex = new Texture2D(itemsPerRow, totalRows, TextureFormat.RGBA32, false, true);
			tex.filterMode = FilterMode.Point;
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.hideFlags = HideFlags.HideAndDontSave;
			int x = 0;
			int y = 0;
			for (int i = 0; i < colors.Length; i++) {
				x = i % itemsPerRow;
				y = totalRows - 1 - Mathf.CeilToInt(i / itemsPerRow);
				tex.SetPixel(x, y, colors[i]);
			}
			for (x++; x < tex.width; x++) {
				tex.SetPixel(x, y, Color.clear);
			}

			tex.Apply();

			return tex;
		}

		void DrawBlackGrid(float startingPointX, float startingPointY, int cellsX, int cellsY, int cellSize) {
			if (blackTexture == null) {
				blackTexture = textureWithColor(Color.black);
			}

			// draw vertical lines
			Rect currentRect = new Rect(startingPointX, startingPointY, 1, cellSize * cellsY);
			for (int i = 0; i <= cellsX; i++) {
				currentRect.x = startingPointX + cellSize * i;
				DrawTexture(blackTexture, currentRect);
			}

			currentRect.x = startingPointX;
			currentRect.height = 1;
			currentRect.width = cellSize * cellsX;

			for (int i = 0; i <= cellsY; i++) {
				currentRect.y = startingPointY + cellSize * i;
				if (i == cellsY)
					currentRect.width++;
				DrawTexture(blackTexture, currentRect);
			}
		}

		GUIStyle tempDrawTextureStyle;
		void DrawTexture(Texture2D texture, Rect rect) {
			if (tempDrawTextureStyle == null) {
				tempDrawTextureStyle = new GUIStyle();
			}
			tempDrawTextureStyle.normal.background = texture;
			EditorGUI.LabelField(rect, "", tempDrawTextureStyle);
		}

		void UpdateActiveSwatch(Color color) {
			swatchTexture.SetPixel(0, 0, color);
			swatchTexture.Apply();
			SwatchEditorGUI.GameViewRepaint();
		}
	}
}