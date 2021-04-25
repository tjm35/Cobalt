using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace Totality.Editor
{
	[CustomEditor(typeof(GridTransform))]
	[CanEditMultipleObjects]
	public class GridTransformEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			foreach (var t in targets.Cast<GridTransform>())
			{
				t.transform.hideFlags = (t.ShouldShowTransform ? HideFlags.None : HideFlags.HideInInspector);
			}

			using (var checkChanges = new EditorGUI.ChangeCheckScope())
			{
				using (new EditorGUI.DisabledScope(targets.Any(t => ((GridTransform)target).GetGrid() == null)))
				{
					LayoutFieldWithLock(new GUIContent("Grid Pos"), serializedObject.FindProperty("m_gridPos"), serializedObject.FindProperty("m_gridPosLocked"));
					LayoutFieldWithLock(new GUIContent("Offset Pos"), serializedObject.FindProperty("m_offsetPos"), serializedObject.FindProperty("m_offsetPosLocked"), showZeroButton: true);
				}
				LayoutFieldWithLock(new GUIContent("Facing"), serializedObject.FindProperty("m_facing"), serializedObject.FindProperty("m_facingLocked"));
				LayoutFieldWithLock(new GUIContent("Offset Angle"), serializedObject.FindProperty("m_offsetAngle"), serializedObject.FindProperty("m_offsetAngleLocked"), showZeroButton: true);

				if (checkChanges.changed)
				{
					serializedObject.ApplyModifiedProperties();
				}
			}
		}

		private void LayoutFieldWithLock(GUIContent label, SerializedProperty fieldProperty, SerializedProperty lockProperty, bool showZeroButton = false)
		{
            float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

			var height = EditorGUI.GetPropertyHeight(fieldProperty.propertyType, label);
			Rect position = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(height));
			{
				label = EditorGUI.BeginProperty(position, label, fieldProperty);
				position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

				float buttonSpacing = 2.0f;
				{
					float buttonWidth = 18.0f;
					Rect lockButton = new Rect(position.x + position.width - buttonWidth, position.y - 1, buttonWidth, baseHeight + 2);
					position.width -= buttonWidth + buttonSpacing;

					GUIContent buttonContent = (lockProperty.boolValue ? EditorGUIUtility.IconContent("LockIcon-On") : EditorGUIUtility.IconContent("LockIcon"));
					GUIStyle lockButtonStyle = new GUIStyle(GUI.skin.button);
					lockButtonStyle.margin = new RectOffset(0,0,0,0);
					lockButtonStyle.padding = new RectOffset(0,0,0,0);

					var oldBGColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.Lerp(oldBGColor, Color.black, lockProperty.boolValue ? 0.5f : 0.0f);

					var oldContentColor = GUI.contentColor;
					GUI.contentColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

					if (GUI.Button(lockButton, buttonContent, lockButtonStyle))
					{
						lockProperty.boolValue = !lockProperty.boolValue;
					}

					GUI.backgroundColor = oldBGColor;
					GUI.contentColor = oldContentColor;
				}

				if (showZeroButton)
				{
					float buttonWidth = 18.0f;
					Rect zeroButton = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, baseHeight);
					position.width -= buttonWidth + buttonSpacing;

					if (GUI.Button(zeroButton, "0"))
					{
						switch (fieldProperty.propertyType)
						{
							case SerializedPropertyType.Vector3:
								fieldProperty.vector3Value = Vector3.zero;
								break;
							case SerializedPropertyType.Float:
								fieldProperty.floatValue = 0.0f;
								break;
							default:
								Debug.LogError("GridTransformEditor: Asked to zero a property we don't have support for.");
								break;
						}
					}
				}

				EditorGUI.PropertyField(position, fieldProperty, GUIContent.none);
				EditorGUI.EndProperty();
			}
		}
	}
}