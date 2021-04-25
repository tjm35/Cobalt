using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public class Grid2D : MonoBehaviour, IGridContext
	{
		public Vector2 CellSize = Vector2.one;

		public Vector3 DirectionToWorld(Vector2Int i_direction)
		{
			return transform.TransformDirection(new Vector3(i_direction.x, 0.0f, i_direction.y));
		}

		public Vector3 DirectionToWorld(Vector3Int i_direction)
		{
			return DirectionToWorld(i_direction.XY());
		}

		public Vector3 GetCellCenterLocal(Vector2Int i_address)
		{
			return new Vector3((i_address.x + 0.5f) * CellSize.x, 0.0f, (i_address.y + 0.5f) * CellSize.y);
		}

		public Vector3 GetCellCenterWorld(Vector2Int i_address)
		{
			return transform.TransformPoint(GetCellCenterLocal(i_address));
		}

		public Vector3 GetCellCenterLocal(Vector3Int i_address)
		{
			return GetCellCenterLocal(i_address.XY());
		}

		public Vector3 GetCellCenterWorld(Vector3Int i_address)
		{
			return GetCellCenterWorld(i_address.XY());
		}

		public Vector3 GetCellSize()
		{
			return new Vector3(CellSize.x, FakeCellSize, CellSize.y);
		}

		public Vector3Int WorldToCell(Vector3 i_worldPos)
		{
			return new Vector3Int
			(
				Mathf.FloorToInt(i_worldPos.x / CellSize.x),
				Mathf.FloorToInt(i_worldPos.z / CellSize.y),
				0
			);
		}

		private const float FakeCellSize = 5.0f;
	}
}