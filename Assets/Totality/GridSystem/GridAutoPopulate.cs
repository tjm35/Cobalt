using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Totality
{
	public class GridAutoPopulate : MonoBehaviour
	{
#if UNITY_EDITOR
		public GameObject m_cellPrototype;
		public int m_xFillMin = 0;
		public int m_xFillMax = 0;
		public int m_yFillMin = 0;
		public int m_yFillMax = 0;
		public bool m_onlyFillMissingCells = true;

		[Button]
		public void PopulateRange()
		{
			var index = GetComponent<GridIndex>() ?? transform.GetComponentInAncestors<GridIndex>();
			if (index == null)
			{
				Debug.LogError("GridAutoPopulate: Could not find GridIndex in object or ancestors.");
				return;
			}
			if (m_cellPrototype == null)
			{
				Debug.LogError("GridAutoPopulate: No cell prototype set up.");
				return;
			}
			if (m_cellPrototype.GetComponent<GridTransform>() == null)
			{
				Debug.LogError("GridAutoPopulate: Cell prototype has no grid transform.");
				return;
			}
			for (int x = m_xFillMin; x <= m_xFillMax; ++x)
			{
				for (int y = m_yFillMin; y <= m_yFillMax; ++y)
				{
					var address = new Vector3Int(x, y, 0);
					if (index.GetComponentAt<MapCell>(address) == null || !m_onlyFillMissingCells)
					{
						var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(m_cellPrototype, transform);
						go.name = $"Cell {x},{y}";
						var gt = go.GetComponent<GridTransform>();
						gt.GridPos = address;
					}
				}
			}
		}
#endif
	}
}