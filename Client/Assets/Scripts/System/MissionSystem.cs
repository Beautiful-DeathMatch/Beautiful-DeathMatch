using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NPOI.SS.Formula.Functions;
using UnityEngine;

/// <summary>
/// 미션은 살짝 보류...
/// 어떤 형태가 될 지 알 수 없다.
/// 우선 MissionComponent를 미션에 해당하는 애들에 붙이고
/// PlayerMissionComponent가 이를 추적하여 클리어 
/// </summary>

public enum ENUM_MISSION_TYPE
{
	None = -1,
	Mission1 = 0,
	Mission2 = 1,
	Mission3 = 2,
	Mission4 = 3,
	Mission5 = 4,
}

public class DynamicMissionData
{
	public MissionTable.MissionData tableData;
    public int currentProgression { get; private set; }

	public DynamicMissionData(MissionTable.MissionData tableData)
	{
		this.tableData = tableData;
		currentProgression = 0;
	}

	public void MissionComplete()
	{
		currentProgression = tableData.maxProgression;
	}

	public void MissionProgressionChange(int progress)
	{
		int newProgression = currentProgression + progress;
		currentProgression = newProgression > tableData.maxProgression ? tableData.maxProgression : newProgression;
	}

	public bool IsMissionCompleted()
	{
		return currentProgression == tableData.maxProgression;
	}

}

public class PlayerMissionSlot
{
	public readonly Dictionary<ENUM_MISSION_TYPE, DynamicMissionData> missions = new();

	public void AddMission(ENUM_MISSION_TYPE missionType, DynamicMissionData dyunamicMissionData)
	{
		missions.Add(missionType, dyunamicMissionData);
	}
}

public class MissionSystem : MonoSystem
{    
	[SerializeField] private MissionTable missionTable;
	private Dictionary<int, PlayerMissionSlot> playerMissionSlotDictionary = new ();

    [SerializeField] private PrefabLinkedUISystem uiSystem = null;

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		// 미션 데이터 딕셔너리 생성
		if (sceneModuleParam is BattleSceneModule.Param battleParam)
		{
			playerMissionSlotDictionary.Clear();

            foreach (var playerInfo in battleParam.playerInfoList)
			{
				PlayerMissionSlot newPlayerMissionSlot = new ();
				playerMissionSlotDictionary.Add(playerInfo.playerId, newPlayerMissionSlot);

				foreach(ENUM_MISSION_TYPE missionType in Enum.GetValues(typeof(ENUM_MISSION_TYPE)))
				{
					if (missionType != ENUM_MISSION_TYPE.None)
					{
						var missionData = missionTable.GetMissionDataByType(missionType);
						newPlayerMissionSlot.AddMission(missionType, new DynamicMissionData(missionData));
					}
				}

            }
        }

	}

	public DynamicMissionData GetMissionData(int playerId, ENUM_MISSION_TYPE missionType)
	{
		if (playerMissionSlotDictionary.TryGetValue(playerId, out var missionSlot) == false)
			return null;

		if (missionSlot.missions.TryGetValue(missionType, out var mission) == false)
			return null;

		return mission;
	}

	public bool TryMissionStart(int playerId, ENUM_MISSION_TYPE missionType)	// 개인 UI 호출을 시스템에서?? <= 이슈 없을 지 논의 필요
	{
		uiSystem.OpenPopup<MissionPopup>(new MissionUIParam(this, playerId, missionType));

		return true;
	}

	public bool TryMissionComplete(int playerId, ENUM_MISSION_TYPE missionType)
	{
		if (playerMissionSlotDictionary.TryGetValue(playerId, out var missionSlot) == false)
			return false;

		if (missionSlot.missions.TryGetValue(missionType, out var mission) == false)
			return false;

		mission.MissionComplete();
		return true;
	}

	public PlayerMissionSlot GetPlayerMissionSlot(int playerId)
	{
		if (playerMissionSlotDictionary.TryGetValue(playerId, out var missionSlot))
			return missionSlot;

		return null;
	}

	public bool IsMissionCompleted(int playerId, ENUM_MISSION_TYPE missionType)
	{
		if (GetPlayerMissionSlot(playerId).missions.TryGetValue(missionType, out var data) == false)
			return false;

		return data.IsMissionCompleted();
	}


}
