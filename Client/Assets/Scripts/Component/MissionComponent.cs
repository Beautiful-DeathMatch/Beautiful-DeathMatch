using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionComponent : MonoComponent<MissionSystem>
{
    [SerializeField] private int missionId = 0;
    [SerializeField] private ENUM_MISSION_TYPE missionType = ENUM_MISSION_TYPE.NONE;
    [SerializeField] private int checkProgress = 0;

    public bool TryCompleteMission(int playerId)
    {
        return true;
    }
}
