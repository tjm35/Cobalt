using BulletFury;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cobalt
{
	public class ScreenContainer : MonoBehaviour
	{
		public UnityEvent PlayerEnterScreen;
		public UnityEvent PlayerExitScreen;
		public GameObject Contents;

		public void OnPlayerEnterScreen()
		{
			m_playerHere = true;
			Contents?.SetActive(true);
			PlayerEnterScreen.Invoke();

			var bc = PlayerManager.Instance.CurrentPlayer?.GetComponent<BulletCollider>();
			if (bc)
			{
				foreach (var manager in transform.GetComponentsInDescendents<BulletManager>())
				{
					bc.AddManagerToBullets(manager);
				}
			}
		}

		public void OnPlayerExitScreen()
		{
			var bc = PlayerManager.Instance.CurrentPlayer?.GetComponent<BulletCollider>();
			if (bc)
			{
				foreach (var manager in transform.GetComponentsInDescendents<BulletManager>())
				{
					bc.RemoveManagerFromBullets(manager);
				}
			}

			PlayerExitScreen.Invoke();
			Contents?.SetActive(false);
			m_playerHere = false;
		}

		private void Start()
		{
			if (!m_playerHere)
			{
				Contents?.SetActive(false);
			}
		}

		private bool m_playerHere = false;
	}
}