using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateComponent : MonoBehaviour
{
	[SerializeField] private Transform cameraTransform;
	[SerializeField] private ThirdPersonController controller = null;

	private float TopClamp = 70.0f;
	private float BottomClamp = -30.0f;

	// cinemachine
	private float _cinemachineTargetYaw = 0.0f;
	private float _cinemachineTargetPitch = 0.0f;

	private void Start()
	{
		_cinemachineTargetPitch = cameraTransform.rotation.eulerAngles.y;
		_cinemachineTargetYaw = cameraTransform.rotation.eulerAngles.x;
	}

	private void OnEnable()
	{
		controller.onRotate += Rotate;
	}

	private void OnDisable()
	{
		controller.onRotate -= Rotate;
	}

	public void Rotate(float yaw, float pitch)
    {
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw + yaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch + pitch, BottomClamp, TopClamp);

		cameraTransform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0);
	}

	private float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f)
		{
			lfAngle += 360f;
		}

		if (lfAngle > 360f)
		{
			lfAngle -= 360f;
		}

		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}
