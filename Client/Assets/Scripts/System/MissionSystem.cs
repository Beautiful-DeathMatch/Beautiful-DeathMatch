using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_MISSION_TYPE
{

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
