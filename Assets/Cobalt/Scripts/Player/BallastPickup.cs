using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class BallastPickup : MonoBehaviour
	{
		public int BallastCount = 1;
		public TMPro.TMP_Text CountText;

		[FMODUnity.EventRef]
		public string PickupSound;

		private void Start()
		{
			if (CountText != null)
			{
				if (BallastCount == 1)
				{
					CountText.gameObject.SetActive(false);
				}
				else
				{
					CountText.text = BallastCount.ToString();
					CountText.gameObject.SetActive(true);
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.GetComponent<Player>())
			{
				PlayerManager.Instance?.State.AddBallast(BallastCount);

				if (!string.IsNullOrEmpty(PickupSound))
				{
					FMODUnity.RuntimeManager.PlayOneShot(PickupSound, transform.position);
				}

				Destroy(gameObject);
			}
		}
	}
}