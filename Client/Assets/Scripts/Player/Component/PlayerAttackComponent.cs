using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{
	[SerializeField] private Transform cameraTransform = null;

	private LayerMask attackLayerMask; // 공격 레이 무시용 레이어 마스크
	private int playerId = -1;

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	private void Awake()
	{
		attackLayerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("Interaction");
	}

	public void Attack(ItemData itemData)
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
}
