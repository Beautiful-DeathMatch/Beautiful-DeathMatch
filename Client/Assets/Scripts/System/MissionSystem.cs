using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NPOI.SS.Formula.Functions;
using UnityEditor.Experimental.GraphView;
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
	public readonly int id = -1;
	public MissionTable.MissionData tableData = null;
	public int currentProgression { get; private set; } = 0;

	public DynamicMissionData(int id = -1, MissionTable.MissionData tableData = null)
	{
		this.id = id;
		this.tableData = tableData;
		currentProgression = 0;
	}

	public DynamicMissionData()
	{
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
	public readonly List<int> missionIds = new();

	public void AddMission(int missionId)
	{
		missionIds.Add(missionId);
	}
}

public class MissionSystem : NetworkSystem
{    
	[SerializeField] private MissionTable missionTable;

    [SerializeField] private PrefabLinkedUISystem uiSystem = null;

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		// 미션 데이터 딕셔너리 생성
		// -> BlackBoard에서

	}

	public DynamicMissionData GetMissionData(int playerId, ENUM_MISSION_TYPE missionType)
	{
		if (blackBoard.TryGetMissionSlot(playerId, out var missionSlot) == false)
			return null;

		foreach(int id in missionSlot.missionIds)
		{
			if (blackBoard.TryGetMissionData(id, out var missionData))
				if (missionData.tableData.key == missionType)
					return missionData;
				else
					continue;
		}

		return null;
	}

	public DynamicMissionData GetMissionData(int missionId)
	{
		if (blackBoard.TryGetMissionData(missionId, out var mission) == false)
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
		if (blackBoard.TryGetMissionSlot(playerId, out var missionSlot) == false)
			return false;

		foreach(int id in missionSlot.missionIds)
		{
			if (blackBoard.TryGetMissionData(id, out var missionData))
				if (missionData.tableData.key == missionType)
				{
					blackBoard.MissionComplete(missionData.id);
					return true;
				}
				else
					continue;
		}

		return false;
	}

	public PlayerMissionSlot GetPlayerMissionSlot(int playerId)
	{
		if (blackBoard.TryGetMissionSlot(playerId, out var missionSlot))
			return missionSlot;

		return null;
	}

	public bool IsMissionCompleted(int playerId, ENUM_MISSION_TYPE missionType)
	{
		if (blackBoard.TryGetMissionSlot(playerId, out var missionSlot) == false)
			return false;

		foreach(int id in missionSlot.missionIds)
		{
			if (blackBoard.TryGetMissionData(id, out var missionData))
				if (missionData.tableData.key == missionType)
				{
					return missionData.IsMissionCompleted();
				}
				else
					continue;
		}

		return false;
	}


}
