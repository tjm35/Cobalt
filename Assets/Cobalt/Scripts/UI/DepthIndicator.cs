using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cobalt
{
	[RequireComponent(typeof(TMP_Text))]
	public class DepthIndicator : MonoBehaviour
	{
		public string Prefix = "";
		public string Suffix = "m below sea level";
		public string Format = "F0";
		public float Multiplier = 1.0f;

		// Start is called before the first frame update
		void Start()
		{
			m_text = GetComponent<TMP_Text>();
		}

		// Update is called once per frame
		void Update()
		{
			var player = PlayerManager.Instance?.CurrentPlayer;
			if (player && WaterManager.Instance)
			{
				var displayDepth = WaterManager.Instance.GetDepth(player.transform) * Multiplier;

				m_text.text = Prefix + displayDepth.ToString(Format) + Suffix;
			}
		}

		private TMP_Text m_text;
	}
}