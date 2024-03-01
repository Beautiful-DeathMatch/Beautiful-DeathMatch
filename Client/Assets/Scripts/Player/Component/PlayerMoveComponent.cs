using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller;
	[SerializeField] private CharacterController characterController;
	[SerializeField] private PlayerGroundCheckComponent groundCheckComponent;
	private Animator animator;

	private Transform cameraTransform;

	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 2.0f;

	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 5.335f;

	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;

	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private float _twistRotationVelocity;

	int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	int _animIDSpeed = Animator.StringToHash("Speed");
	int _animIDSpeedX = Animator.StringToHash("SpeedX");
	int _animIDSpeedY = Animator.StringToHash("SpeedY");

	bool isAiming = false;
	[SerializeField] private float maxTwistViewAngle;

	Vector3 lastInputDirection = new Vector3(0f,0f,0f);

	private void Awake()
	{
		cameraTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		animator = GetComponentInChildren<Animator>();
		controller.onMove += Move;
		controller.onAiming += OnAiming;
	}

	private void OnDisable()
	{
		controller.onMove -= Move;
		controller.onAiming -= OnAiming;
	}

	private void OnAiming(bool isAiming)
	{
		this.isAiming = isAiming;
	}

	public void Move(bool isSprint, bool analogMovement, Vector2 inputMoveVec)
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = isSprint && !isAiming ? SprintSpeed : MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (inputMoveVec == Vector2.zero)
			targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = analogMovement ? inputMoveVec.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset ||
			currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
				Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
		if (_animationBlend < 0.01f) _animationBlend = 0f;

		// normalise input direction
		Vector3 inputDirection = new Vector3(inputMoveVec.x, 0.0f, inputMoveVec.y).normalized;
		if(inputDirection.magnitude > 0)
			lastInputDirection = new Vector3(inputDirection.x, 0f, inputDirection.z);

		_targetRotation = (lastInputDirection.z >= 0 ? Mathf.Atan2(lastInputDirection.x, lastInputDirection.z) : Mathf.Atan2(-lastInputDirection.x, -lastInputDirection.z)  )
								* Mathf.Rad2Deg + cameraTransform.eulerAngles.y; // z < 0 인 경우 뒤돌기 방지를 위한 처리 추가
		float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
			RotationSmoothTime);

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (inputMoveVec != Vector2.zero)
		{
			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}
		else
		{
			float angleGap = cameraTransform.eulerAngles.y - transform.rotation.eulerAngles.y;
			angleGap = angleGap < -180f ? angleGap + 360f : (angleGap > 180f ? angleGap - 360f : angleGap);
			if (angleGap > maxTwistViewAngle)
			{
				float _targetTwistRotation = cameraTransform.eulerAngles.y - maxTwistViewAngle;
				rotation = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _targetTwistRotation, ref _twistRotationVelocity, RotationSmoothTime);
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}
			else if (angleGap < -maxTwistViewAngle)
			{
				float _targetTwistRotation = cameraTransform.eulerAngles.y + maxTwistViewAngle;
				rotation = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _targetTwistRotation, ref _twistRotationVelocity, RotationSmoothTime);
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}
			Debug.Log(new Vector3(cameraTransform.eulerAngles.y, transform.rotation.eulerAngles.y, angleGap));
		}

		Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

		// move the player
		characterController.Move(targetDirection.normalized * ((lastInputDirection.z >= 0 ?_speed : -_speed) * Time.deltaTime) +
					 new Vector3(0.0f, groundCheckComponent._verticalVelocity, 0.0f) * Time.deltaTime); // z < 0 인 경우 speed 는 음수 (앞을 보고 뒤로 가므로)

		// characterController.SimpleMove(targetDirection.normalized * (_speed * Time.deltaTime) +
			//			 new Vector3(0.0f, groundCheckComponent._verticalVelocity, 0.0f) * Time.deltaTime);

		animator.SetFloat(_animIDSpeed, _animationBlend);
		animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
		animator.SetFloat(_animIDSpeedX, _animationBlend * lastInputDirection.x);
		animator.SetFloat(_animIDSpeedY, _animationBlend * lastInputDirection.z);
	}
}
