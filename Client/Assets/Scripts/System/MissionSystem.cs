using System.Collections;
using System.Collections.Generic;
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
	Interaction = 0,
}

public class MissionData
{
    public readonly ENUM_MISSION_TYPE missionType;     // 미션 타입
    public int currentProgression { get; private set; }
    public readonly int maxProgression;  // 미션 최대 진행도
}

public class MissionSystem : MonoSystem
{    
    private Dictionary<int, MissionData> missionDataDictionary = new Dictionary<int, MissionData>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		// 미션 데이터 딕셔너리 생성

	}
}
