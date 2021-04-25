using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	[AddComponentMenu("Grid/Grid Transform")]
	[ExecuteAlways]
	public class GridTransform : MonoBehaviour
	{
		#region Public Properties
		public Vector3Int GridPos
		{
			get => m_gridPos;
			set { m_gridPos = value; UpdateTransformPos(); UpdateIndex(); }
		}
			
		public Vector3 OffsetPos
		{
			get => m_offsetPos;
			set { m_offsetPos = value; UpdateTransformPos(); }
		}

		public int Facing
		{
			get => m_facing;
			set { m_facing = (value + 4) % 4; UpdateTransformFacing(); }
		}

		public float OffsetAngle
		{
			get => m_offsetAngle;
			set { m_offsetAngle = value; UpdateTransformFacing(); }
		}

		public Vector2Int Forward => TransformDirection(Vector2Int.up);
		public Vector3Int Forward3 => new Vector3Int(Forward.x, Forward.y, 0);

		public GridIndex Index => m_index;
		#endregion

		#region Public Methods
		public IGridContext GetGrid()
		{
			if (m_host != null)
			{
				return m_host;
			}
			else
			{
				var context = transform.GetComponentInAncestors<IGridContext>();
				if (context != null)
				{
					return context;
				}
				var grid = transform.GetComponentInAncestors<Grid>();
				if (grid != null)
				{
					return new GridContextUnityGridWrapper(grid);
				}
				return null;
			}
		}

		public Vector2Int TransformDirection(Vector2Int i_direction)
		{
			return TransformDirection(i_direction, m_facing);
		}

		public static Vector2Int TransformDirection(Vector2Int i_direction, int i_facing)
		{
			switch (i_facing)
			{
				case 0:
				default:
					return i_direction;
				case 1:
					return new Vector2Int(i_direction.y, -i_direction.x);
				case 2:
					return -i_direction;
				case 3:
					return new Vector2Int(-i_direction.y, i_direction.x);
			}
		}

		public static int FacingDelta(Vector3Int i_directionStart, Vector3Int i_directionEnd)
		{
			return FacingDelta(new Vector2Int(i_directionStart.x, i_directionStart.y), new Vector2Int(i_directionEnd.x, i_directionEnd.y));
		}

		public static int FacingDelta(Vector2Int i_directionStart, Vector2Int i_directionEnd)
		{
			int fakeCross = i_directionStart.y * i_directionEnd.x - i_directionStart.x * i_directionEnd.y;
			int fakeDot = i_directionStart.x * i_directionEnd.x + i_directionStart.y * i_directionEnd.y;

			if (fakeDot < 0)
			{
				return ((fakeCross + 4) % 4) - 2;
			}
			else
			{
				return fakeCross;
			}
		}
		#endregion

		#region Context Menu
		[ContextMenu("Force Update Transform")]
		public void UpdateTransform()
		{
			UpdateTransformPos();
			UpdateTransformFacing();
		}

#if UNITY_EDITOR
		[ContextMenu("Hide Transform")]
		public void HideTransform()
		{
			ShouldShowTransform = false;
		}

		[ContextMenu("Show Transform")]
		public void ShowTransform()
		{
			ShouldShowTransform = true;
		}
#endif
		#endregion

		#region Unity Lifetime Functions
		private void Start()
		{
			if (Application.isPlaying)
			{
				m_host = GetGrid();
				Debug.Assert(m_host != null, "GridTransform must be an immediate child of a Grid gameobject");
				m_index = transform.GetComponentInAncestors<GridIndex>();
				UpdateIndex();
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				// Update is only used in the editor.
				enabled = false;
				return;
			}

			#if UNITY_EDITOR
			var grid = GetGrid();
			if (grid != null && transform.localPosition != GetExpectedPos())
			{
				Debug.Log($"Position update of {gameObject.name}, {transform.localPosition} != {GetExpectedPos()}");
				var targetPosition = transform.localPosition;
				if (!m_gridPosLocked)
				{
					GridPos = grid.WorldToCell(targetPosition);
				}
				if (!m_offsetPosLocked)
				{
					OffsetPos = targetPosition - grid.GetCellCenterLocal(GridPos);
				}
				if (m_gridPosLocked && m_offsetPosLocked)
				{
					UpdateTransformPos();
				}
			}
			if (transform.localRotation != GetExpectedRot())
			{
				Debug.Log($"Rotation update of {gameObject.name}, {transform.localRotation} != {GetExpectedRot()}");
				var targetYRot = transform.localRotation.eulerAngles.y;
				if (!m_facingLocked)
				{
					Facing = Mathf.RoundToInt(targetYRot / 90.0f);
				}
				if (!m_offsetAngleLocked)
				{
					OffsetAngle = targetYRot - (Facing * 90.0f);
				}
				if (m_facingLocked && m_offsetAngleLocked)
				{
					UpdateTransformFacing();
				}
			}
			#endif
		}

		private void OnDestroy()
		{
			if (m_index)
			{
				m_index.Deindex(this);
			}
#if UNITY_EDITOR
			ShowTransform();
#endif
		}

		private void OnValidate()
		{
			m_facing = m_facing % 4;
			UpdateTransform();
		}

		private void Reset()
		{
			//Debug.Log("Reset called");
			Vector3 currentPos = transform.localPosition;
			//Debug.Log($"CurrentPos: {currentPos}");
			var grid = GetGrid();
			//Debug.Log(grid == null ? "No grid" : "Grid found");
			if (grid != null)
			{
				Vector3Int bestGridPos = grid.WorldToCell(currentPos);
				//Debug.Log($"BestGridPos: {bestGridPos}");
				GridPos = bestGridPos;
				OffsetPos = currentPos - transform.localPosition;
			}
			else
			{
				Debug.LogWarning("GridTransform.Reset: No Grid found in ancestors.");
			}
			float angle = transform.localEulerAngles.y;
			//Debug.Log($"Angle: {angle}");
			Facing = Mathf.RoundToInt(angle / 90.0f);
			OffsetAngle = angle - transform.localEulerAngles.y;
		}
		#endregion

		#region Private Methods
		private void UpdateIndex()
		{
			if (m_index)
			{
				m_index.UpdateIndex(this);
				Debug.Assert(System.Linq.Enumerable.Contains(m_index.GetComponentsAt<GridTransform>(GridPos), this));
			}
		}

		private void UpdateTransformPos()
		{
			transform.localPosition = GetExpectedPos();
		}

		private Vector3 GetExpectedPos()
		{
			return (GetGrid()?.GetCellCenterLocal(GridPos) ?? Vector3.zero) + OffsetPos;
		}

		private void UpdateTransformFacing()
		{
			transform.localRotation = GetExpectedRot();
		}

		private Quaternion GetExpectedRot()
		{
			return Quaternion.Euler(transform.localEulerAngles.x, ((float)Facing * 90.0f + m_offsetAngle), transform.localEulerAngles.z);
		}
		#endregion

		[SerializeField]
		private Vector3Int m_gridPos = Vector3Int.zero;
		[SerializeField]
		private Vector3 m_offsetPos = Vector3.zero;
		[SerializeField]
		private int m_facing = 0;
		[SerializeField]
		private float m_offsetAngle = 0.0f;

#if UNITY_EDITOR
		[SerializeField]
		private bool m_gridPosLocked = true;
		[SerializeField]
		private bool m_offsetPosLocked = false;
		[SerializeField]
		private bool m_facingLocked = false;
		[SerializeField]
		private bool m_offsetAngleLocked = false;

		public bool ShouldShowTransform { get; set; } = false;
#endif
		private IGridContext m_host;
		private GridIndex m_index;
	}
}