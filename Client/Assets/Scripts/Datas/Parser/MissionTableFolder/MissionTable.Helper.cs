using System;
using System.Collections.Generic;
using UnityEngine;

public partial class MissionTable
{

    public MissionData GetMissionDataByType(ENUM_MISSION_TYPE missionType)
    {
        if (missionDataDictionary.TryGetValue(missionType, out var missionData) == false)
            return null;

        return missionData;
    }

}