using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cobalt
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class MovingEntity : MonoBehaviour
	{
		public IPathProvider Path;
		public float TargetAcceptDistance = 0.1f;
		public float MaxSpeed = 10.0f;
		public float MoveSmoothTime = 0.1f;

		public void Start()
		{
			if (Path == null)
			{
				Path = transform.GetComponentInAncestors<IPathProvider>();
			}
			m_target = transform.position;
			m_rigidBody = GetComponent<Rigidbody2D>();
		}

		public void Update()
		{
			if (Path != null)
			{
				if (Vector3.Distance(transform.position, m_target) < TargetAcceptDistance)
				{
					Path?.GetNextTarget(ref m_target, out Path);
				}
			}
			Vector3 velocity = m_rigidBody.velocity;
			Vector3.SmoothDamp(transform.position, m_target, ref velocity, MoveSmoothTime, MaxSpeed);
			m_rigidBody.velocity = velocity;

		}

		private Rigidbody2D m_rigidBody;
		private Vector3 m_target;
	}
}