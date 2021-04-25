using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public class BackAndForthPath : MonoBehaviour, IPathProvider
	{
		public Vector3 HalfExtent;

		public void GetNextTarget(ref Vector3 io_target, out IPathProvider o_provider)
		{
			if (Vector3.Dot(io_target - transform.position, HalfExtent) > 0)
			{
				io_target = transform.position - HalfExtent;
			}
			else
			{
				io_target = transform.position + HalfExtent;
			}
			o_provider = this;
		}
	}
}