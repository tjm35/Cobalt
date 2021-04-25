using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public class CellDebug : MonoBehaviour
	{
		public Color m_cellColour = Color.white;
		public bool m_alwaysDrawDebug = false;
		public float m_vOffset = 0.01f;
		public float m_insetDebug = 0.04f;
		public float m_fillAlphaMultiplier = 0.05f;

		private void OnDrawGizmos()
		{
			if (m_alwaysDrawDebug)
			{
				DrawDebugGizmos();
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (m_alwaysDrawDebug)
			{
				DrawDebugGizmos();
			}
		}

		private void DrawDebugGizmos()
		{
			var gt = GetComponent<GridTransform>();
			var grid = gt?.GetGrid();
			if (grid != null)
			{
				Vector3 debugCenter = grid.GetCellCenterWorld(gt.GridPos) + gt.OffsetPos + m_vOffset * Vector3.up;
				Vector3 boxSize = grid.GetCellSize() - 2.0f * m_insetDebug * Vector3.one;
				boxSize.y = 0.0f;
				Color fillColour = m_cellColour;
				fillColour.a *= m_fillAlphaMultiplier;
				Gizmos.color = fillColour;
				Gizmos.DrawCube(debugCenter, boxSize);
				Gizmos.color = m_cellColour;
				Gizmos.DrawWireCube(debugCenter, boxSize);
				Gizmos.color = Color.white;
#if UNITY_EDITOR
				UnityEditor.Handles.color = m_cellColour;
				UnityEditor.Handles.Label(debugCenter, $"{gt.GridPos.x}, {gt.GridPos.y}");
				UnityEditor.Handles.color = Color.white;
#endif
			}
		}
	}
}