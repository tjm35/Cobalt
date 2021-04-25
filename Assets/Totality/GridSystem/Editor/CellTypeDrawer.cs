using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Totality.Editor
{
	[CustomPropertyDrawer(typeof(CellType))]
	public class CellTypeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var settings = CellTypeSettings.instance;
			var nameProperty = property.FindPropertyRelative("m_name");
			var current = nameProperty.stringValue;
			var options = settings.CellTypes?.Select(t => new GUIContent(t)) ?? new GUIContent[0];
			if (!(settings.CellTypes?.Contains(current) ?? false))
			{
				options = options.Append(new GUIContent(current));
			}
			var formalOptions = options.ToArray();

			var index = options.TakeWhile(c => c.text != current).Count();
			index = EditorGUI.Popup(position, label, index, formalOptions);
			nameProperty.stringValue = formalOptions[index].text;
			property.FindPropertyRelative("m_id").intValue = Animator.StringToHash(nameProperty.stringValue);
		}
	}
}