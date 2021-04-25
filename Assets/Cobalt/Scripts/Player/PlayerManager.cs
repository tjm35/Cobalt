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

#if UNITY_EDITOR
		public bool UseDevelopmentSpawn;
		public GameObject DevelopmentSpawnPoint;
		public bool DevelopmentImmortal;
		public bool Immortal => DevelopmentImmortal;
#else
		public bool Immortal => false;
#endif

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
#if UNITY_EDITOR
			if (UseDevelopmentSpawn && DevelopmentSpawnPoint != null)
			{
				SpawnPoint = DevelopmentSpawnPoint;
				if (DevelopmentSpawnPoint.GetComponent<SpawnPoint>())
				{
					State.BallastCount = DevelopmentSpawnPoint.GetComponent<SpawnPoint>().ExpectedBallast;
				}
			}
#endif
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
					player.transform.position = new Vector3(SpawnPoint.transform.position.x, SpawnPoint.transform.position.y, 0.0f);
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