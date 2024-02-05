using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	bool TryTakeDamage(int attackerPlayerId, int damage);
}

/// <summary>
/// 데미지 및 회복을 관리합니다.
/// </summary>
public class PlayerStatusComponent : MonoComponent<StatusSystem>, IDamageable
{
	public bool TryTakeDamage(int attackerPlayerId, int damage)
	{
		if (System.TryChangeHealth(attackerPlayerId, -damage) == false)
			return false;

		return true;
	}
}
