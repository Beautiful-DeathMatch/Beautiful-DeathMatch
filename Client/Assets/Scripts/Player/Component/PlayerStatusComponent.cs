using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void RequestTakeDamage(int attackerPlayerId, int damage);
}

public static class AnimatorHelper
{
	public static bool IsCurrentState(this Animator animator, int layerIndex, params string[] stateNames)
	{
		foreach(var state in stateNames)
		{
			if (IsCurrentState(animator, layerIndex, state))
				return true;
		}

		return false;
	}

	public static bool IsCurrentState(this Animator animator, int layerIndex, string stateName)
	{
		return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
	}
}


/// <summary>
/// 데미지 및 회복을 관리합니다.
/// </summary>
public class PlayerStatusComponent : NetworkComponent<StatusSystem>, IDamageable
{
	[SerializeField] private Animator animator = null;
	[SerializeField] private int hitLayerIndex = 2;

	[SerializeField] private string standHitStateName = "Stand Hit";
	[SerializeField] private string deadStateName = "Dead";

	private int HitHash = Animator.StringToHash("IsHit"); 
	private int playerId = -1;

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}
	
	[Command]
	public void RequestTakeDamage(int attackerPlayerId, int damageAmount)
	{
		if (attackerPlayerId == playerId)
			return;

		if (System.TryChangeHealth(playerId, -damageAmount) == false)
			return;
	}

	private void LateUpdate()
	{
		if (netIdentity.isOwned == false)
			return;

		if (animator.IsCurrentState(hitLayerIndex, deadStateName))
			return;

		if (System.IsHitState(playerId))
			return;

		if (animator.GetBool(HitHash))
			return;

		animator.SetTrigger(HitHash);
		System.ChangeHitState(playerId, false);
	}
}
