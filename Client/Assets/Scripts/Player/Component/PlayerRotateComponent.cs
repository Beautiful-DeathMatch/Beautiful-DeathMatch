using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRotateComponent : MonoBehaviour
{
	[SerializeField] private Transform cameraTransform;
	[SerializeField] private ThirdPersonController controller = null;
	
	[SerializeField] private float TopClamp = 70.0f;
	[SerializeField] private float BottomClamp = -30.0f;

	private Rig[] rigs = null;

	private float _cinemachineTargetYaw = 0.0f;
	private float _cinemachineTargetPitch = 0.0f;

	private bool isAiming = false;

	private void Start()
	{
		_cinemachineTargetPitch = cameraTransform.rotation.eulerAngles.y;
		_cinemachineTargetYaw = cameraTransform.rotation.eulerAngles.x;
	}

	private void OnEnable()
	{
		rigs = GetComponentsInChildren<Rig>();
		controller.onRotate += Rotate;
		controller.onAiming += OnAiming;
	}

	private void OnDisable()
	{
		controller.onRotate -= Rotate;
		controller.onAiming -= OnAiming;
	}

	private void OnAiming(bool isAiming)
	{
		this.isAiming = isAiming;
	}

	private void LateUpdate()
	{
		UpdateRigWeightByAngle(cameraTransform.localRotation.eulerAngles.y);
	}

	private void UpdateRigWeightByAngle(float yAngleVector)
	{
		foreach (var rig in rigs)
		{
			if (yAngleVector > 90.0f && yAngleVector <= 180.0f)
			{
				rig.weight = Mathf.Lerp(1, 0, (yAngleVector - 90.0f) / 90.0f);
			}
			else if (yAngleVector <= -180.0f && yAngleVector > -270.0f)
			{
				rig.weight = Mathf.Lerp(1, 0, (yAngleVector + 270.0f) / 90.0f);
			}
			else if (yAngleVector <= -90.0f && yAngleVector > -180.0f)
			{
				rig.weight = Mathf.Lerp(0, 1, (yAngleVector + 180.0f) / 90.0f);
			}
			else if (yAngleVector > 180.0f && yAngleVector <= 270.0f)
			{
				rig.weight = Mathf.Lerp(0, 1, (yAngleVector - 180.0f) / 90.0f);
			}
			else
			{
				rig.weight = 1;
			}
		}
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
