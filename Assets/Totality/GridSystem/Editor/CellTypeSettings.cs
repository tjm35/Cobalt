using NaughtyAttributes.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[FilePath("ProjectSettings/CellTypeSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class CellTypeSettings : ScriptableSingleton<CellTypeSettings>
{
	public IEnumerable<string> CellTypes => m_cellTypes;

	public bool IsKnown(string i_cellType)
	{
		return m_cellTypes.Contains(i_cellType);
	}

	public void Add(string i_cellType)
	{
		if (!IsKnown(i_cellType))
		{
			m_cellTypes[m_cellTypes.Length] = i_cellType;
			Save();
		}
	}

	public void Save()
	{
		Save(true);
	}
	/*
	private void OnDisable()
	{
		Save();
	}
	*/ 
	[SerializeField]
	private string[] m_cellTypes;
}

public class CellTypeSettingsProvider : SettingsProvider
{
	CellTypeSettingsProvider() : base("Project/Grid Cell Types", SettingsScope.Project)
	{

	}

	private class DummyArray : ScriptableObject
	{
		public string[] m_cellTypes;
	}

	public override void OnGUI(string searchContext)
	{
		using (var changeCheck = new EditorGUI.ChangeCheckScope())
		{
			var settings = new SerializedObject(CellTypeSettings.instance);
			settings.Update();

			var typesProperty = settings.FindProperty("m_cellTypes");

			// For some reason ScriptableSingletons serialize with editable == false,
			// which makes EditorGUILayout.PropertyField readonly. We use an intermediary
			// object to get around this.
			var dummyObject = ScriptableObject.CreateInstance<DummyArray>();
			{
				var dummy = new SerializedObject(dummyObject);
				dummy.CopyFromSerializedProperty(typesProperty);
				var dummyTypesProperty = dummy.FindProperty("m_cellTypes");
				EditorGUILayout.PropertyField(dummyTypesProperty);

				settings.CopyFromSerializedProperty(dummyTypesProperty);
			}
			Object.DestroyImmediate(dummyObject);


			if (changeCheck.changed)
			{
				settings.ApplyModifiedProperties();
				CellTypeSettings.instance.Save();
			}
		}
	}

	[SettingsProvider]
	public static SettingsProvider Create()
	{
		return new CellTypeSettingsProvider();
	}
}