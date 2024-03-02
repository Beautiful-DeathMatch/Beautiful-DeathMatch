using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;
using Cinemachine;

public class PlayerAttackComponent : NetworkBehaviour
{
	[SerializeField] private Transform cameraOriginTransform = null;
	[SerializeField] private Transform aimTargetTransform = null;
	[SerializeField] private ThirdPersonController controller = null;

	private Cinemachine3rdPersonFollow cinemachineFollow = null;
	private Transform muzzleTransform = null;
	private Animator animator;
	private int AimHash = Animator.StringToHash("Aim");
	private bool isAiming = false;

	private LayerMask attackLayerMask; // 공격 레이 무시용 레이어 마스크
	private int playerId = -1;

	[SerializeField] private float defaultCamDistance = 5f;
	[SerializeField] private float aimingCamDistance = 2f;
	[SerializeField] private float defaultCamOffsetY = 0.5f;
	[SerializeField] private float aimingCamOffsetY = 0.2f;
	[SerializeField] private float aimingSmoothTime = 0.5f;
	private float _distanceVelocity;
	private float _offsetYVelocity;

	private void OnEnable()
	{
		animator = GetComponentInChildren<Animator>();
		controller.onAiming += OnAiming;
	}

	private void OnDisable()
	{
		controller.onAiming -= OnAiming;
	}

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	public void SetMuzzle(Transform muzzle)
	{
		muzzleTransform = muzzle;
	}

	public void SetFollowCamera(CinemachineVirtualCamera virtualCamera)
	{
		cinemachineFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
		cinemachineFollow.CameraDistance = 5.0f;
	}

	private void Awake()
	{
		attackLayerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("Interaction");
	}

	public void TrySlashAttack(DynamicItemData itemData)
    {
		OnNoAimAttack(cameraOriginTransform.position, cameraOriginTransform.forward, itemData.tableData.attackDistance, itemData.tableData.hpAmount);
	}

	public void TryShotAttack(DynamicItemData itemData)
	{
		if (isAiming)
		{
			OnAimAttack(muzzleTransform.position, aimTargetTransform.position, itemData.tableData.hpAmount);
		}
		else
		{
			OnNoAimAttack(muzzleTransform.position, muzzleTransform.forward, itemData.tableData.attackDistance, itemData.tableData.hpAmount);
		}
	}

	[Command]
	private void OnNoAimAttack(Vector3 origin, Vector3 dir, float distance, int damageAmount)
	{
		Debug.DrawRay(origin, dir * distance, Color.red, 3f);

		if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, attackLayerMask)) // 충돌 감지 시
		{
			var damageable = hit.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.RequestTakeDamage(playerId, damageAmount);
			}
		}
	}

	[Command]
	private void OnAimAttack(Vector3 origin, Vector3 target, int damageAmount)
	{
		Debug.DrawLine(origin, target, Color.red, 3f);

		if (Physics.Linecast(origin, target, out RaycastHit hit, attackLayerMask))
		{
			var damageable = hit.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.RequestTakeDamage(playerId, damageAmount);
			}
		}
	}

	private void OnAiming(bool isAiming)
	{
		this.isAiming = isAiming;
		animator.SetBool(AimHash, isAiming);

		float distance;
		float offsetY;
		if (isAiming)
		{
			distance = Mathf.SmoothDamp(cinemachineFollow.CameraDistance, aimingCamDistance, ref _distanceVelocity, aimingSmoothTime);
			offsetY = Mathf.SmoothDamp(cinemachineFollow.ShoulderOffset.y, aimingCamOffsetY, ref _offsetYVelocity, aimingSmoothTime);
		}
		else
		{
			distance = Mathf.SmoothDamp(cinemachineFollow.CameraDistance, defaultCamDistance, ref _distanceVelocity, aimingSmoothTime);
			offsetY = Mathf.SmoothDamp(cinemachineFollow.ShoulderOffset.y, defaultCamOffsetY, ref _offsetYVelocity, aimingSmoothTime);
		}

		cinemachineFollow.CameraDistance = distance;
		cinemachineFollow.ShoulderOffset.y = offsetY;
	}
}
