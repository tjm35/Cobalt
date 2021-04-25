using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cobalt
{
	[RequireComponent(typeof(Image))]
	public class BuoyancyIndicator : MonoBehaviour
	{
		public Sprite AscendingSprite;
		public Sprite DescendingSprite;
		public Sprite NeutralSprite;

		public Color AscendingColor = Color.white;
		public Color DescendingColor = Color.white;
		public Color NeutralColor = Color.white;

		public float NeutralTolerance = 1.0f;

		private void Start()
		{
			m_image = GetComponent<Image>();
		}

		private void Update()
		{
			var player = PlayerManager.Instance?.CurrentPlayer;
			if (player)
			{
				float currentDepth = player.transform.position.y;
				float neutralDepth = player.GetComponent<PlayerController>().NeutralDepth;

				if (Mathf.Abs(currentDepth - neutralDepth) < NeutralTolerance)
				{
					m_image.sprite = NeutralSprite;
					m_image.color = NeutralColor;
				}
				else if (currentDepth > neutralDepth)
				{
					m_image.sprite = DescendingSprite;
					m_image.color = DescendingColor;
				}
				else
				{
					m_image.sprite = AscendingSprite;
					m_image.color = AscendingColor;
				}
			}
		}

		private Image m_image;
	}
}