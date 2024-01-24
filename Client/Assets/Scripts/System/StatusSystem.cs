using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusData
{
	public readonly int maxHealthPoint = 100;

	public StatusData(int maxHealthPoint)
	{
		this.maxHealthPoint = maxHealthPoint;
	}
}

public class DynamicStatusData : StatusData
{
	public int currentHealthPoint { get; private set; }

	public DynamicStatusData(StatusData statusData) : base(statusData.maxHealthPoint)
	{
		currentHealthPoint = statusData.maxHealthPoint;
	}

	public void ChangeHealth(int amount)
	{
		int resultHealthPoint = currentHealthPoint + amount;
		currentHealthPoint = resultHealthPoint > 0 ? resultHealthPoint : 0;
	}
}

public class StatusSystem : MonoSystem
{
    Dictionary<int, DynamicStatusData> statusDataDictionary = new Dictionary<int, DynamicStatusData>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		// 스테이터스 데이터 딕셔너리 생성


	}

	public bool TryChangeHealth(int playerId, int amount)
	{
		if (statusDataDictionary.TryGetValue(playerId, out DynamicStatusData statusData) == false)
			return false;

		if (statusData.currentHealthPoint <= 0)
			return false;

		statusData.ChangeHealth(amount);
		return true;
	}
}
