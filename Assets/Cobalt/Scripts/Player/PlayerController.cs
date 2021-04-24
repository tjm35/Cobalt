using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cobalt
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour
	{
		public float MoveSpeed = 40.0f;
		public float MoveSmoothTime = 0.5f;
		public InputAction MoveAction;

		private void Start()
		{
			m_rigidBody = GetComponent<Rigidbody2D>();
		}

		private void OnEnable()
		{
			MoveAction.Enable();
		}

		private void OnDisable()
		{
			MoveAction.Disable();
		}

		private void FixedUpdate()
		{
			var targetVelocity = MoveDirection * MoveSpeed;
			var velocity = m_rigidBody.velocity;
			m_rigidBody.velocity = Vector2.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref velocity, MoveSmoothTime);
		}

		private Vector2 MoveDirection => MoveAction.ReadValue<Vector2>();

		private Rigidbody2D m_rigidBody;
	}
}