using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Totality
{
	public class GridIndex : MonoBehaviour
	{
		public IEnumerable<GridTransform> GetAt(Vector3Int i_address)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return transform.GetComponentsInDescendents<GridTransform>().Where(gt => gt?.GridPos == i_address);
			}
			else
#endif
			{
				List<GridTransform> list = null;
				m_index.TryGetValue(i_address, out list);
				return list ?? Enumerable.Empty<GridTransform>();
			}
		}

		public IEnumerable<T> GetComponentsAt<T>(Vector3Int i_address)
		{
			return GetAt(i_address).SelectMany(gt => gt.GetComponents<T>()).Where(c => c != null);
		}

		public T GetComponentAt<T>(Vector3Int i_address)
		{
			return GetComponentsAt<T>(i_address).FirstOrDefault();
		}

		public void UpdateIndex(GridTransform i_gt)
		{
			Deindex(i_gt);
			AddAt(i_gt, i_gt.GridPos);
			Debug.Assert(GetAt(i_gt.GridPos).Count() > 0, "GetAt returns empty list.");
			Debug.Assert(GetComponentAt<GridTransform>(i_gt.GridPos) != null, "GetComponentAt returns null.");
			Debug.Assert(GetComponentsAt<GridTransform>(i_gt.GridPos).Contains(i_gt), "GetComponentAt returns wrong address.");
		}

		public void Deindex(GridTransform i_gt)
		{
			Vector3Int oldAddress;
			if (m_reverseLookup.TryGetValue(i_gt, out oldAddress))
			{
				RemoveAt(i_gt, oldAddress);
			}
		}

		private void RemoveAt(GridTransform i_gt, Vector3Int i_addr)
		{
			Debug.Assert(m_index.ContainsKey(i_addr));
			Debug.Assert(m_index[i_addr] != null);
			m_index[i_addr].Remove(i_gt);
			m_reverseLookup.Remove(i_gt);
		}

		private void AddAt(GridTransform i_gt, Vector3Int i_addr)
		{
			if (!m_index.ContainsKey(i_addr))
			{
				m_index[i_addr] = new List<GridTransform>();
				Debug.Assert(m_index.TryGetValue(i_addr, out var list), "TryGetValue failed.");
			}
			Debug.Assert(m_index[i_addr].Contains(i_gt) == false);
			m_index[i_addr].Add(i_gt);
			Debug.Assert(m_index[i_addr].Contains(i_gt) == true);
			m_reverseLookup[i_gt] = i_addr;
		}

		private Dictionary<Vector3Int, List<GridTransform>> m_index = new Dictionary<Vector3Int, List<GridTransform>>();
		private Dictionary<GridTransform, Vector3Int> m_reverseLookup = new Dictionary<GridTransform, Vector3Int>();
	}
}