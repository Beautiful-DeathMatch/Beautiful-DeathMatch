using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;

public class PlayerAttackComponent : NetworkBehaviour
{
	[SerializeField] private Transform mussleTransform = null;
	[SerializeField] private Transform cameraOriginTransform = null;

	[SerializeField] private Animator animator;
	[SerializeField] private ThirdPersonController controller = null;

	private Transform shotTargetTransform = null;
	private int AimHash = Animator.StringToHash("Aim");

	private LayerMask attackLayerMask; // 공격 레이 무시용 레이어 마스크
	private int playerId = -1;

	private void OnEnable()
	{		
		controller.onStartAim += OnStartAim;
		controller.onEndAim += OnEndAim;
	}

	private void OnDisable()
	{
		controller.onStartAim -= OnStartAim;
		controller.onEndAim -= OnEndAim;
	}

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	public void SetShotTarget(Transform target)
	{
		shotTargetTransform = target;
	}

	private void Awake()
	{
		attackLayerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("Interaction");
	}

	public void TrySlashAttack(DynamicItemData itemData)
    {
		OnSlashAttack(cameraOriginTransform.position, cameraOriginTransform.forward, itemData.tableData.attackDistance, itemData.tableData.hpAmount);
	}

	public void TryShotAttack(DynamicItemData itemData)
	{
		OnShotAttack(mussleTransform.position, shotTargetTransform.position, itemData.tableData.hpAmount);
	}

	[Command]
	private void OnSlashAttack(Vector3 origin, Vector3 dir, float distance, int damageAmount)
	{
		Debug.DrawRay(origin, dir * distance, Color.red, 1f);

		if (Physics.Raycast(mussleTransform.position, cameraOriginTransform.forward, out RaycastHit hit, distance, attackLayerMask)) // 충돌 감지 시
		{
			var damageable = hit.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.RequestTakeDamage(playerId, damageAmount);
			}
		}
	}

	[Command]
	private void OnShotAttack(Vector3 origin, Vector3 target, int damageAmount)
	{
		Debug.DrawLine(origin, target, Color.red, 15f);

		if (Physics.Linecast(origin, target, out RaycastHit hit, attackLayerMask))
		{
			var damageable = hit.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.RequestTakeDamage(playerId, damageAmount);
			}
		}
	}

	private void OnStartAim()
	{
		animator.SetBool(AimHash, true);
	}

	private void OnEndAim()
	{
		animator.SetBool(AimHash, false);
	}
}
