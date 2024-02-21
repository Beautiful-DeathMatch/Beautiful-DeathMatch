using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerAttackComponent : MonoBehaviour
{
	[SerializeField] private Transform cameraTransform = null;
	[SerializeField] private Animator animator;
	[SerializeField] private ThirdPersonController controller = null;
	
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

	private void Awake()
	{
		attackLayerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("Interaction");
	}

	public void Attack(DynamicItemData itemData)
    {
		Debug.DrawRay(cameraTransform.position, cameraTransform.forward * itemData.tableData.attackDistance, Color.red, 1f);
		
		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, itemData.tableData.attackDistance, attackLayerMask)) // 충돌 감지 시
		{
			var damageable = hit.transform.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TryTakeDamage(playerId, itemData.tableData.hpAmount);
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
