using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class Player : MonoBehaviour
	{
		[EventRef]
		public string DeathSFX;

		public void Damage()
		{
			m_dying = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (!m_registered)
			{
				if (PlayerManager.Instance)
				{
					PlayerManager.Instance.RegisterPlayer(this);
					ScreenManager.Instance?.CurrentScreen?.OnPlayerEnterScreen();
					m_registered = true;
				}
			}
			if (m_dying)
			{
				if (!string.IsNullOrEmpty(DeathSFX))
				{
					RuntimeManager.PlayOneShot(DeathSFX, transform.position);
				}
				Destroy(gameObject);
			}
		}

		private void OnDisable()
		{
			if (m_registered)
			{
				ScreenManager.Instance?.CurrentScreen?.OnPlayerExitScreen();
				PlayerManager.Instance?.UnregisterPlayer(this);
				m_registered = false;
			}
		}

		private void OnDestroy()
		{
			if (m_registered)
			{
				PlayerManager.Instance?.UnregisterPlayer(this);
				m_registered = false;
			}
		}

		private bool m_registered = false;
		private bool m_dying = false;
	}
}