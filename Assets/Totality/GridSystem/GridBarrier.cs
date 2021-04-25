using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public class GridBarrier : MonoBehaviour
	{
		public bool IsActive = true;
		public Vector2Int LocalDirection = new Vector2Int(1, 0);
		public Vector2Int Direction => m_gridTransform.TransformDirection(LocalDirection);

		private void Start()
		{
			m_gridTransform = GetComponent<GridTransform>();
		}

		private void OnDrawGizmosSelected()
		{
			var gridTransform = GetComponent<GridTransform>();
			if (gridTransform)
			{
				var transformedDirection = gridTransform.TransformDirection(LocalDirection);

				Vector3 swizzledDirection = gridTransform.GetGrid().DirectionToWorld(transformedDirection);
				Vector3 scaledSwizzled = Vector3.Scale(gridTransform.GetGrid().GetCellSize(), swizzledDirection);
				Vector3 swizzledPerpendicular = gridTransform.GetGrid().DirectionToWorld(new Vector2Int(transformedDirection.y, transformedDirection.x));
				Vector3 scaledSwizzledPerp = Vector3.Scale(gridTransform.GetGrid().GetCellSize(), swizzledPerpendicular);

				var oldCol = Gizmos.color;
				Gizmos.color = IsActive ? Color.red : Color.green;
				Gizmos.DrawWireCube(gridTransform.GetGrid().GetCellCenterWorld(gridTransform.GridPos) + 0.5f * scaledSwizzled, scaledSwizzledPerp);
				Gizmos.color = oldCol;
			}
		}

		private GridTransform m_gridTransform;
	}
}