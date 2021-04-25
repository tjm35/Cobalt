using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	[System.Serializable]
	public class CellType
	{
		public int ID => m_id;

		[SerializeField]
		public int m_id;
#if UNITY_EDITOR
		[SerializeField]
		public string m_name;
#endif
	}
}