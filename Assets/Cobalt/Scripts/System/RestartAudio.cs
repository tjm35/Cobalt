using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class RestartAudio : MonoBehaviour
	{
		public bool RetryContinually = true;

		public bool DoRestart()
		{
			FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
			m_audioReady = (FMODUnity.RuntimeManager.CoreSystem.mixerResume() == FMOD.RESULT.OK);
			return m_audioReady;
		}

		// Update is called once per frame
		void Update()
		{
			if (!m_audioReady && RetryContinually)
			{
				DoRestart();
			}
		}

		private bool m_audioReady = false;
	}
}