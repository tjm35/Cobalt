using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public class GridContextUnityGridWrapper : IGridContext
	{
		public GridContextUnityGridWrapper(Grid i_baseGrid)
		{
			m_baseGrid = i_baseGrid;
		}

		public Vector3 GetCellCenterLocal(Vector3Int i_address)
		{
			return m_baseGrid.GetCellCenterLocal(i_address);
		}

		public Vector3 GetCellCenterWorld(Vector3Int i_address)
		{
			return m_baseGrid.GetCellCenterWorld(i_address);
		}

		public Vector3 GetCellCenterLocal(Vector2Int i_address)
		{
			return m_baseGrid.GetCellCenterLocal(i_address.ToVector3Int());
		}

		public Vector3 GetCellCenterWorld(Vector2Int i_address)
		{
			return m_baseGrid.GetCellCenterWorld(i_address.ToVector3Int());
		}

		public Vector3Int WorldToCell(Vector3 i_worldPos)
		{
			return m_baseGrid.WorldToCell(i_worldPos);
		}

		public Vector3 DirectionToWorld(Vector2Int i_direction)
		{
			return DirectionToWorld(new Vector3Int(i_direction.x, i_direction.y, 0));
		}

		public Vector3 DirectionToWorld(Vector3Int i_direction)
		{
			return m_baseGrid.transform.TransformDirection(Grid.Swizzle(m_baseGrid.cellSwizzle, i_direction));
		}

		public Vector3 GetCellSize()
		{
			return m_baseGrid.cellSize;
		}

		private Grid m_baseGrid;
	}
}