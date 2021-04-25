using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public class MapCell : MonoBehaviour, ISerializationCallbackReceiver
	{
		public CellType CellType => m_cellTypeObj;

		public bool IsA(string i_cellType)
		{
			return IsA(Animator.StringToHash(i_cellType));
		}

		public bool IsA(int i_cellType)
		{
			return CellType.ID == i_cellType;
		}

		public void OnBeforeSerialize()
		{
			
		}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			if (string.IsNullOrEmpty(m_cellTypeObj.m_name))
			{
				m_cellTypeObj.m_name = m_cellType;
				m_cellTypeObj.m_id = Animator.StringToHash(m_cellType);
			}
#endif
		}

		[SerializeField]
		[HideInInspector]
		private string m_cellType;
		[SerializeField]
		[InspectorName("Cell Type")]
		private CellType m_cellTypeObj;
	}
}