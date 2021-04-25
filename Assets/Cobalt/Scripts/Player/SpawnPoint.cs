using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	[RequireComponent(typeof(Animator))]
	public class SpawnPoint : MonoBehaviour
	{
		public int ExpectedBallast = 0;
		public int MinBallast = 0;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.GetComponent<Player>())
			{
				PlayerManager.Instance.SpawnPoint = gameObject;
				PlayerManager.Instance.State.BallastCount = Mathf.Max(PlayerManager.Instance.State.BallastCount, MinBallast);
			}
		}

		private void Start()
		{
			m_animator = GetComponent<Animator>();
			m_activeID = Animator.StringToHash("Active");
		}

		private void Update()
		{
			m_animator.SetBool(m_activeID, PlayerManager.Instance?.SpawnPoint == gameObject);
		}

		private Animator m_animator;
		private int m_activeID;
	}
}