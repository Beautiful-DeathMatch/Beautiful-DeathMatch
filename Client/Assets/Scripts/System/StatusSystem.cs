using System.Collections;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Dml.Diagram;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

public enum STATE
{
	None,
	Ready,
	Live,
	Dead,
	Revive,
	Clear
}

public class StatusData
{
	public readonly int maxHealthPoint = 100;
	public readonly float maxReviveTime = 3f;

	public StatusData(int maxHealthPoint = 100)
	{
		this.maxHealthPoint = maxHealthPoint;
		this.maxReviveTime = 3;
	}
}

public class DynamicStatusData : StatusData			// 이거 상속 안해도 될거같은디 아니면 Item도 상속을 시키는 방법도?
{
	public int currentHealthPoint { get; private set; }
	public bool isShield { get; private set; } = false;
	public STATE state { get; private set; } = STATE.Live;
	public float currentReviveTime { get; private set; }

	public DynamicStatusData(StatusData statusData) : base(statusData.maxHealthPoint)
	{
		currentHealthPoint = statusData.maxHealthPoint;
		currentReviveTime = 0f;
	}

	public void ChangeHealth(int amount)
	{
		int resultHealthPoint = currentHealthPoint + amount;
		currentHealthPoint = resultHealthPoint > 0 ? resultHealthPoint : 0;
		if (currentHealthPoint == 0 && state == STATE.Live)
		{
			ChangeState(STATE.Dead);
			currentReviveTime = maxReviveTime;
		}
	}

	public void ChangeShield(bool value)
	{
		isShield = value;
	}

	public void ChangeState(STATE newState)
	{
		state = newState;
	}

	public void ChangeReviveTime(float time)
	{
		float resultReviveTime = currentReviveTime + time;
		currentReviveTime = resultReviveTime > 0 ? resultReviveTime : 0;
	}

}

public class StatusSystem : MonoSystem
{
	/// <summary>
	/// 동기화 필요!
	/// 1. 상태 데이터 목록 : playerId - DynamicStatusData
	/// 2. NPC 상태 데이터 목록 (디버깅용 or 추후 NPC 확장성 고려) : npcStatusId - DynamicStatusData
	/// </summary>
    Dictionary<int, DynamicStatusData> statusDataDictionary = new Dictionary<int, DynamicStatusData>();
    Dictionary<int, DynamicStatusData> npcStatusDataDictionary = new Dictionary<int, DynamicStatusData>();
	StatusData defaultPlayerStatusData = new();
	StatusData defaultNpcStatusData = new(1000);

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		// 스테이터스 데이터 딕셔너리 생성
		if (sceneModuleParam is BattleSceneModule.Param battleParam)
		{
			statusDataDictionary.Clear();

            foreach (var playerInfo in battleParam.playerInfoList)
			{
				statusDataDictionary.Add(playerInfo.playerId, new DynamicStatusData(defaultPlayerStatusData));
            }
        }

		// Field 의 NPC Component 확인 후 등록
		npcStatusDataDictionary.Clear();
		foreach (var npcStatus in FindObjectsOfType<NpcStatusComponent>())
		{
			int npcId = npcStatus.GetNpcId();
			if(npcStatusDataDictionary.TryGetValue(npcId, out var data))
			{
				Debug.Log("NPC Status 등록 실패: NPC ID 가 중복됩니다");
				continue;
			}
			npcStatusDataDictionary.Add(npcId, new DynamicStatusData(defaultNpcStatusData));
			Debug.Log("NPC Status 등록 완료");
		}

	}

	public bool TryChangeHealth(int playerId, int amount)
	{
		if (statusDataDictionary.TryGetValue(playerId, out DynamicStatusData statusData) == false)
			return false;

		if (statusData.currentHealthPoint <= 0)
			return false;

		if (statusData.isShield)
			return false;

		statusData.ChangeHealth(amount);
		return true;
	}

	public bool TryNpcChangeHealth(int npcId, int amount)
	{
		if (npcStatusDataDictionary.TryGetValue(npcId, out DynamicStatusData statusData) == false)
			return false;

		if (statusData.currentHealthPoint <= 0)
			return false;

		statusData.ChangeHealth(amount);
		return true;
	}

	public int GetHealth (int playerId)
	{
		if(statusDataDictionary.TryGetValue(playerId, out var data))
			return data.currentHealthPoint;

		return -1;
	}

	public int GetNpcHealth (int NpcId)
	{
		if(npcStatusDataDictionary.TryGetValue(NpcId, out var data))
			return data.currentHealthPoint;

		return -1;
	}

	public STATE GetState (int playerId)
	{
		if(statusDataDictionary.TryGetValue(playerId, out var data))
			return data.state;

		return STATE.None;
	}

	public bool TryStartRevive(int playerId)
	{
		if (statusDataDictionary.TryGetValue(playerId, out DynamicStatusData statusData) == false)
			return false;

		statusData.ChangeState(STATE.Revive);

		// 부활 지점 선택 로직이 들어갈 예정

		TryCompleteRevive(playerId);
		return true;
	}

	public bool TryCompleteRevive(int playerId)
	{
		if (statusDataDictionary.TryGetValue(playerId, out DynamicStatusData statusData) == false)
			return false;

		statusData.ChangeState(STATE.Live);
		return true;
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

		foreach(var status in statusDataDictionary.Values)
		{
			if(status.state == STATE.Dead)
			{
				
				status.ChangeReviveTime(-Time.deltaTime);
			}
		}
    }

}
