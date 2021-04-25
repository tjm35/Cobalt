using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality
{
	public static class VectorIntExtensions
	{
		public static Vector3Int ToVector3Int(this Vector2Int i_this)
		{
			return new Vector3Int(i_this.x, i_this.y, 0);
		}

		public static Vector2Int XY(this Vector3Int i_this)
		{
			return new Vector2Int(i_this.x, i_this.y);
		}

		public static Vector3Int Dot(Vector3Int i_a, Vector3Int i_b)
		{
			return new Vector3Int(i_a.x * i_b.x, i_a.y * i_b.y, i_a.z * i_b.z);
		}

		public static Vector3Int Sign(this Vector3Int i_this)
		{
			return new Vector3Int(Math.Sign(i_this.x), Math.Sign(i_this.y), Math.Sign(i_this.z));
		}
	}
}