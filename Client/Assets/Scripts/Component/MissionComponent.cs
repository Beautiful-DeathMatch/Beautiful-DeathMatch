using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionComponent : MonoComponent<MissionSystem>
{
	[SerializeField] public ENUM_MISSION_TYPE missionType = ENUM_MISSION_TYPE.None;

	[SerializeField] private int missionId = 0;
    [SerializeField] private int checkProgress = 0;

    public bool TryCompleteMission(ENUM_MISSION_TYPE missionType, int playerId)
    {
        if (this.missionType != missionType)
            return false;

        return true;
    }
}
