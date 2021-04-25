using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

namespace Totality
{
	[RequireComponent(typeof(GridTransform))]
	public class CellUtility : MonoBehaviour
	{
#if UNITY_EDITOR
		public GameObject m_newPrototype;
		public Transform m_spawnHost;

		[Button]
		public void SpawnHere()
		{
			if (m_newPrototype == null)
			{
				Debug.LogError("CellUtility: No new prototype set up.");
				return;
			}
			if (m_newPrototype.GetComponent<GridTransform>() == null)
			{
				Debug.LogError("CellUtility: New prototype has no grid transform.");
				return;
			}
			var go = (GameObject)PrefabUtility.InstantiatePrefab(m_newPrototype, m_spawnHost ?? transform.parent);
			var gt = go.GetComponent<GridTransform>();
			gt.GridPos = GetComponent<GridTransform>().GridPos;
			gt.OffsetPos = GetComponent<GridTransform>().OffsetPos;
		}

		[Button]
		public void ReplaceThis()
		{
			if (m_newPrototype == null)
			{
				Debug.LogError("CellUtility: No new prototype set up.");
				return;
			}
			if (m_newPrototype.GetComponent<GridTransform>() == null)
			{
				Debug.LogError("CellUtility: New prototype has no grid transform.");
				return;
			}
			var go = (GameObject)PrefabUtility.InstantiatePrefab(m_newPrototype, transform.parent);
			go.name = gameObject.name;
			var gt = go.GetComponent<GridTransform>();
			gt.GridPos = GetComponent<GridTransform>().GridPos;
			gt.OffsetPos = GetComponent<GridTransform>().OffsetPos;

			runInEditMode = true;
			m_queueDestroy = true;
		}

		[Button]
		public void DoAutoBuilds()
		{
			foreach (var caw in GetComponents<CellAutoWall>().Concat(transform.GetComponentsInDescendents<CellAutoWall>()))
			{
				caw.DestroyChildObjects();
				caw.CreateWalls();
			}
		}

		private void Update()
		{
			if (m_queueDestroy)
			{
				DestroyImmediate(gameObject);
			}
			else
			{
				runInEditMode = false;
			}
		}


		private bool m_queueDestroy = false;
#endif
	}
}