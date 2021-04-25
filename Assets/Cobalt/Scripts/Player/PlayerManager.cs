using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	[RequireComponent(typeof(PlayerState))]
	public class PlayerManager : MonoBehaviour
	{
		public static PlayerManager Instance { get; private set; }

		public Player CurrentPlayer => m_player;
		public PlayerState State { get; private set; }

		public Grid Grid;
		public GameObject SpawnPoint;
		public GameObject PlayerPrefab;

		public void RegisterPlayer(Player i_player)
		{
			Debug.Assert(m_player == null, "ScreenManager is not set up to handle multiple players at once.");
			m_player = i_player;
		}

		public void UnregisterPlayer(Player i_player)
		{
			Debug.Assert(m_player == i_player);
			m_player = null;
		}

		private void OnEnable()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		private void OnDisable()
		{
			Debug.Assert(Instance == this);
			Instance = null;
		}

		private void Start()
		{
			State = GetComponent<PlayerState>();
		}

		private void Update()
		{
			if (m_player == null)
			{
				if (m_respawnTimer > 0)
				{
					m_respawnTimer--;
				}
				else
				{
					var player = Instantiate(PlayerPrefab);
					player.transform.position = SpawnPoint.transform.position;
					player.transform.rotation = Quaternion.identity;
					m_respawnTimer = 5;
				}
			}
			else
			{
				m_respawnTimer = 5;
			}
		}

		private int m_respawnTimer = 0;
		private Player m_player;
	}
}