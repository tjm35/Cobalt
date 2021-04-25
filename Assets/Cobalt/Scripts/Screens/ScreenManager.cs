using System.Collections;
using System.Collections.Generic;
using Totality;
using UnityEngine;

namespace Cobalt
{
	[RequireComponent(typeof(Grid))]
	[RequireComponent(typeof(GridIndex))]
	public class ScreenManager : MonoBehaviour
	{
		public static ScreenManager Instance { get; private set; }

		public ScreenContainer CurrentScreen { get; private set; }
		public GridTransform ScreenCamera;

		public void Start()
		{
			m_grid = GetComponent<Grid>();
			m_index = GetComponent<GridIndex>();
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

		public void Update()
		{
			if (PlayerManager.Instance.CurrentPlayer != null)
			{
				Vector3Int playerLocation = m_grid.WorldToCell(PlayerManager.Instance.CurrentPlayer.transform.position);
				ScreenContainer desiredScreen = m_index.GetComponentAt<ScreenContainer>(playerLocation);
				if (m_currentLocation == null || m_currentLocation.Value != playerLocation)
				{
					//Debug.Log($"Changing to {desiredScreen} at {playerLocation}.");
					var oldScreen = CurrentScreen;
					EnterScreen(desiredScreen);
					if (ScreenCamera)
					{
						ScreenCamera.GridPos = playerLocation;
					}
					m_currentLocation = playerLocation;
					CurrentScreen = desiredScreen;
					LeaveScreen(oldScreen);
				}
			}
		}

		public void LeaveScreen(ScreenContainer i_screen)
		{
			if (i_screen)
			{
				i_screen.OnPlayerExitScreen();
			}
		}

		public void EnterScreen(ScreenContainer i_screen)
		{
			if (i_screen)
			{
				i_screen.OnPlayerEnterScreen();
			}
		}

		private Grid m_grid;
		private GridIndex m_index;
		private Vector3Int? m_currentLocation;
	}
}