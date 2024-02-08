using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class MissionUIParam : UIParam
{
    public MissionSystem missionSystem { get; private set; } = null;
    public int playerId { get; private set; } = 0;
    public ENUM_MISSION_TYPE missionType { get; private set; } = ENUM_MISSION_TYPE.None;

    public MissionUIParam(MissionSystem missionSystem = null, int playerId = 0, ENUM_MISSION_TYPE missionType = ENUM_MISSION_TYPE.None)
    {
        this.missionSystem = missionSystem;
        this.playerId = playerId;
        this.missionType = missionType;
    }
}


[PopupAttribute("MissionPopup")]
public class MissionPopup : UIPopup
{
    MissionSystem missionSystem;

    [SerializeField] MissionTable missionTable;
    [SerializeField] StringTable stringTable;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI contentText;

    private int playerId;
    private ENUM_MISSION_TYPE missionType;

	protected override void OnOpen(UIParam param = null)
	{
		base.OnOpen(param);
        
        if (param is MissionUIParam missionUIParam)
        {
            missionSystem = missionUIParam.missionSystem;
            playerId = missionUIParam.playerId;
            missionType = missionUIParam.missionType;

            MissionTable.MissionData missionData = missionTable.GetMissionDataByType(missionType);
            titleText.text = stringTable.GetStringByKey(missionData.nameKey);
            contentText.text = stringTable.GetStringByKey(missionData.contentKey);
        }
	}

    protected override void OnClose()
    {
        
    }

    public void OnClickComplete()
    {
        missionSystem.TryMissionComplete(playerId, missionType);
        OnClickClose();
        return; 
    }

}
