using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheckComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller = null;

	[SerializeField] private float GroundedOffset = -0.14f;
	[SerializeField] private float GroundedRadius = 0.28f;

	[SerializeField] private LayerMask GroundLayers;

	// timeout deltatime
	private float _jumpTimeoutDelta = 0.0f;
	private float _fallTimeoutDelta = 0.0f;

	private float _terminalVelocity = 53.0f;

	private float JumpHeight = 1.2f;
	private float Gravity = -15.0f;

	private float JumpTimeout = 0.50f;
	private float FallTimeout = 0.15f;

	private bool isFallTimeout;

	public float _verticalVelocity { get; private set; }


	private void Start()
	{
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	private void OnEnable()
	{
		controller.IsGrounded += IsGrounded;
		controller.IsFallTimeout += IsFalledTimeout;
		controller.IsFirstJumpTrigger += IsFirstJumpTrigger;
		controller.onJump += OnJump;
	}

	private void OnDisable()
	{
		controller.IsGrounded -= IsGrounded;
		controller.IsFallTimeout -= IsFalledTimeout;
		controller.IsFirstJumpTrigger -= IsFirstJumpTrigger;
		controller.onJump -= OnJump;
	}

	private bool IsFalledTimeout()
	{
		return isFallTimeout;
	}

	private void Update()
    {
		JumpAndGravity();
	}

	private bool IsGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
			transform.position.z);

		return Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
			QueryTriggerInteraction.Ignore);
	}

	private bool IsFirstJumpTrigger()
	{
		return _jumpTimeoutDelta <= 0.0f;
	}

	private void OnJump()
	{
		_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
	}

	private void JumpAndGravity()
	{
		isFallTimeout = false;

		if (IsGrounded())
		{
			_fallTimeoutDelta = FallTimeout;

			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			_jumpTimeoutDelta = JumpTimeout;

			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				isFallTimeout = true;
			}
		}

		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}
}
