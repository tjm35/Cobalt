using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class WaterManager : MonoBehaviour
	{
		public static WaterManager Instance;

		public float SeaLevelHeight = 48.0f;

		public float GetDepthAt(Vector3 i_point)
		{
			return SeaLevelHeight - i_point.y;
		}

		public float GetDepth(Transform i_pos)
		{
			return GetDepthAt(i_pos.position);
		}

		public void OnEnable()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		public void OnDisable()
		{
			Debug.Assert(Instance == this);
			Instance = null;
		}
	}
}