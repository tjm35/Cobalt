using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cobalt
{
	public class Player : MonoBehaviour
	{
		[EventRef]
		public string DeathSFX;

		public InputAction RestartAction;

		public void Damage()
		{
			m_dying = true;
		}

		private void Start()
		{
			RestartAction.performed += (InputAction.CallbackContext _cc) => Damage();
		}

		public void Destroy()
		{
			if (!string.IsNullOrEmpty(DeathSFX))
			{
				RuntimeManager.PlayOneShot(DeathSFX, transform.position);
			}
			Destroy(gameObject);
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
				if (PlayerManager.Instance == null || PlayerManager.Instance.Immortal == false)
				{
					Destroy();
				}
			}
		}



		private void OnEnable()
		{
			RestartAction.Enable();
		}

		private void OnDisable()
		{
			RestartAction.Disable();

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