using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Totality
{
	public class CellAutoWall : MonoBehaviour
	{
#if UNITY_EDITOR
		public string NeighbourType;
		public GameObject WallPrefab;

		[ContextMenu("Create Walls")]
		public void CreateWalls()
		{
			var gt = GetComponent<GridTransform>();
			if (gt == null)
			{
				gt = transform.GetComponentInAncestors<GridTransform>();
			}
			var currentPos = gt.GridPos;

			CreateWall(currentPos + Vector3Int.left, 0.0f);
			CreateWall(currentPos + Vector3Int.up, 90.0f);
			CreateWall(currentPos + Vector3Int.right, 180.0f);
			CreateWall(currentPos + Vector3Int.down, 270.0f);
		}

		private void CreateWall(Vector3Int i_address, float i_angle)
		{
			CreateWall(GetIndex()?.GetComponentAt<MapCell>(i_address), i_angle);
		}

		private void CreateWall(MapCell i_cell, float i_angle)
		{
			if (i_cell != null && i_cell.IsA(NeighbourType) && WallPrefab != null)
			{
				var obj = (GameObject)PrefabUtility.InstantiatePrefab(WallPrefab);
				obj.transform.parent = transform;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.AngleAxis(i_angle, Vector3.up);
			}
		}

		[ContextMenu("Destroy All Child Objects")]
		public void DestroyChildObjects()
		{
			List<GameObject> children = gameObject.GetChildren().ToList();
			foreach (var c in children)
			{
				DestroyImmediate(c);
			}
		}

		private GridIndex GetIndex()
		{
			return transform.GetComponentInAncestors<GridIndex>();
		}
#endif
	}
}