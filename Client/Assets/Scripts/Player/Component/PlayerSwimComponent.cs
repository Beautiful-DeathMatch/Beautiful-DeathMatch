using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller = null;
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
	private float _rotationX;

	int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	int _animIDSpeed = Animator.StringToHash("Speed");

	private int InWaterHash = Animator.StringToHash("InWater");
	private bool isInWater = false;

	private void Awake()
	{
		cameraTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		animator = GetComponentInChildren<Animator>();
		controller.IsInWater += IsInWater;
		controller.onSwim += Swim;
	}

	private void OnDisable()
	{
		controller.IsInWater -= IsInWater;
		controller.onSwim -= Swim;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			isInWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			isInWater = false;
		}
	}

	private bool IsInWater()
	{
		return isInWater;
	}

	private void CheckInWater()
	{
		if (IsInWater())
			animator.SetBool(InWaterHash, true);
		else
			animator.SetBool(InWaterHash, false);
	}

	private void Update() 
	{
		CheckInWater();
	}

	public void Swim(bool isSprint, bool analogMovement, Vector2 inputMoveVec)
	{
		//camera
		Vector3 camLook = cameraTransform.forward;

		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = isSprint ? SprintSpeed : MoveSpeed;

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

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (inputMoveVec != Vector2.zero)
		{
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
				RotationSmoothTime);

			_rotationX = (cameraTransform.eulerAngles.x > 180f ? cameraTransform.eulerAngles.x-360f : cameraTransform.eulerAngles.x) * inputDirection.z;
			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(_rotationX, rotation, 0f);
		}

		Vector3 targetDirection = Quaternion.Euler(_rotationX, _targetRotation, 0f) * Vector3.forward;

		// move the player
		characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime));


		animator.SetFloat(_animIDSpeed, _animationBlend);
		animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
	}


}
