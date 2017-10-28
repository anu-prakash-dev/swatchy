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
			Swatch swatch = swatchyColor.swatch;
			Color color = swatchyColor.color;
			if (swatchTexture == null) {
				swatchTexture = textureWithColor(color);
			}

			var swatchProperty = property.FindPropertyRelative("_swatch");
			var colorIndexProperty = property.FindPropertyRelative("_colorIndex");

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var swatchSize 				= EditorGUIUtility.singleLineHeight;
			var keySize 				= EditorGUIUtility.singleLineHeight*1.25f;
			var spacing 				= EditorGUIUtility.singleLineHeight * 0.5f;
			//var toggleSize				= EditorGUIUtility.singleLineHeight;
			var toggleSize = 0;
			var swatchObjectPositionX 	= swatch == null ? position.x : position.x + swatchSize + keySize + toggleSize + spacing * 2;
			//var swatchObjectWidth = swatch == null ? position.width : position.width - swatchSize - keySize - spacing * 2;
			var swatchObjectWidth		= position.width - swatchObjectPositionX + position.x;
			var swatchObjectRect 		= new Rect(swatchObjectPositionX, position.y, swatchObjectWidth, EditorGUIUtility.singleLineHeight);
			var swatchRect 				= new Rect(position.x, position.y, swatchSize, EditorGUIUtility.singleLineHeight);
			var colorIndexRect 			= new Rect(swatchRect.position.x + swatchRect.width + spacing, position.y, keySize, EditorGUIUtility.singleLineHeight);

			EditorGUI.BeginProperty(position, label, property);


			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;



			// Draw Swatch object
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(swatchObjectRect, swatchProperty, GUIContent.none);
			if (EditorGUI.EndChangeCheck()) {
				property.serializedObject.ApplyModifiedProperties();
				swatchyColor.swatch = swatchyColor._swatch; // hack which calls observer pattern
				UpdateActiveSwatch(swatchyColor.color);
			}

			if (swatch != null) {
				// Draw Color Field
				if (DrawTextureButton(swatchTexture, swatchRect)) {
					paletteOpen = !paletteOpen && swatch != null && swatch.colors != null && swatch.colors.Length > 0;
				}
				DrawBlackGrid(swatchRect.x, swatchRect.y, 1, 1, (int)EditorGUIUtility.singleLineHeight);

				// Draw Color index text field
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(colorIndexRect, colorIndexProperty, GUIContent.none);
				if (EditorGUI.EndChangeCheck()) {
					property.serializedObject.ApplyModifiedProperties();
					swatchyColor.colorIndex = colorIndexProperty.intValue; // hack which calls observer pattern
					UpdateActiveSwatch(swatchyColor.color);
				}
				// Draw Toggle
				//EditorGUI.PropertyField(usingSwatchGroupToggleR, usingSwatchGroupProperty, GUIContent.none);
				//usingSwatchGroupProperty.boolValue = EditorGUI.Toggle(usingSwatchGroupToggleR, usingSwatchGroupProperty.boolValue);

				if (paletteOpen) {
					if (palleteTexture == null) {
						palleteTexture = textureWithColors(swatch.colors);
					}
					var textureRect = new Rect(swatchRect.x, swatchRect.y + EditorGUIUtility.singleLineHeight + 3, palleteTexture.width * EditorGUIUtility.singleLineHeight, palleteTexture.height * EditorGUIUtility.singleLineHeight);
					DrawTexture(palleteTexture, textureRect);
					DrawBlackGrid(textureRect.x, textureRect.y, palleteTexture.width, palleteTexture.height, (int)EditorGUIUtility.singleLineHeight);

					// listen to click
					Event e = Event.current;
					if (IsClickInRect(textureRect)) {
						Vector2 rectClickPosition = e.mousePosition - textureRect.position;
						int cellXIndex = (int)(rectClickPosition.x / EditorGUIUtility.singleLineHeight);
						int cellYIndex = (int)(rectClickPosition.y / EditorGUIUtility.singleLineHeight);
						int colorIndex = cellYIndex * palleteTexture.width + cellXIndex;
						colorIndexProperty.intValue = colorIndex;
						property.serializedObject.ApplyModifiedProperties();
						swatchyColor.colorIndex = colorIndex; //  calls observer pattern
						UpdateActiveSwatch(swatchyColor.color);
					}
					else if (IsClick()) {
						paletteOpen = false;
						EditorUtility.SetDirty(property.serializedObject.targetObject); // Repaint

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

		bool DrawTextureButton(Texture2D texture, Rect rect) {
			bool buttonPressed = GUI.Button(rect, "", GUIStyle.none);
			DrawTexture(texture, rect);
			return buttonPressed;
		}

		void UpdateActiveSwatch(Color color) {
			swatchTexture.SetPixel(0, 0, color);
			swatchTexture.Apply();
			SwatchEditorGUI.GameViewRepaint();
		}

		bool IsClick() {
			Event e = Event.current;
			return e != null && e.isMouse && e.button == 0;
		}

		bool IsClickInRect(Rect rect) {
			Event e = Event.current;
			return e != null && e.isMouse && e.button == 0 && rect.Contains(e.mousePosition);
		}
	}
}