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
		public float MoveAccel = 80.0f;
		public InputAction MoveAction;

		public float NeutralDepth => PlayerManager.Instance?.State.NeutralDepth ?? 0.0f;
		public float DepthWindowHalfSize = 40.0f;

		public float VerticalDragFactor = 0.5f;

		[FMODUnity.EventRef]
		public string SplashdownSound;

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
			if (WaterManager.Instance)
			{
				float depth = WaterManager.Instance.GetDepth(transform);

				if (depth < 0.0f)
				{
					// We're out of water - no control.
					m_rigidBody.gravityScale = 1.0f;
				}
				else
				{
					// We're in water.
					m_rigidBody.gravityScale = 0.0f;

					// Horizontally we just set a target velocity.
					var targetVelocity = MoveDirection * MoveSpeed;
					var velocity = m_rigidBody.velocity;
					var xVel = velocity.x;
					velocity.x = Mathf.SmoothDamp(m_rigidBody.velocity.x, targetVelocity.x, ref xVel, MoveSmoothTime);
					m_rigidBody.velocity = velocity;

					// Vertically our movement is a combination of forces:
					var depthWindowFactor = MoveAccel / DepthWindowHalfSize;
					var springForceY = (depth - NeutralDepth) * depthWindowFactor;
					var motiveForceY = MoveDirection.y * MoveAccel;
					var dragForceY = -velocity.y * Mathf.Abs(velocity.y) * VerticalDragFactor;

					m_rigidBody.AddForce(Vector2.up * (springForceY + motiveForceY + dragForceY));
				}
			}
		}

		private void Update()
		{
			float depth = WaterManager.Instance.GetDepth(transform);
			if (m_lastDepth.HasValue && m_lastDepth.Value * depth < 0.0f && m_splashCooldown < 0.1f)
			{
				if (!string.IsNullOrEmpty(SplashdownSound))
				{
					FMODUnity.RuntimeManager.PlayOneShot(SplashdownSound);
				}
				m_splashCooldown = 1.0f;
			}
			m_splashCooldown = Mathf.Max(0.0f, m_splashCooldown - Time.deltaTime);
			m_lastDepth = depth;
		}

		private Vector2 MoveDirection => MoveAction.ReadValue<Vector2>();

		private Rigidbody2D m_rigidBody;
		private float? m_lastDepth;
		private float m_splashCooldown = 0.0f;
	}
}