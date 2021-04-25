using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public interface IGridContext
	{
		Vector3 GetCellCenterLocal(Vector3Int i_address);
		Vector3 GetCellCenterWorld(Vector3Int i_address);
		Vector3 GetCellCenterLocal(Vector2Int i_address);
		Vector3 GetCellCenterWorld(Vector2Int i_address);
		Vector3Int WorldToCell(Vector3 i_worldPos);

		Vector3 DirectionToWorld(Vector2Int i_direction);
		Vector3 DirectionToWorld(Vector3Int i_direction);

		Vector3 GetCellSize();

	}
}