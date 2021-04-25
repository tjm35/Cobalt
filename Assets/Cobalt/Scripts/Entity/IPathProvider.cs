using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	public interface IPathProvider
	{
		void GetNextTarget(ref Vector3 io_target, out IPathProvider o_provider);
	}
}