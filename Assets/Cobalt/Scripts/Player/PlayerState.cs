using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class PlayerState : MonoBehaviour
	{
		public int BallastCount = 0;

		public float BallastEffect = 4.0f;

		public float NeutralDepth => BallastEffect * (float)BallastCount;

		public void AddBallast(int i_count = 1)
		{
			BallastCount += i_count;
			BallastCount = Mathf.Max(BallastCount, 0);
		}
	}
}