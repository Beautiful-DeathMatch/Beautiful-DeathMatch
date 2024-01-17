using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData
{
    public MissionData(int _ownerID, int _missionType, int _progression, int __maxProgression)
    {
        ownerID = _ownerID;
        missionType = _missionType;
        progression = _progression;
        maxProgression = __maxProgression;
        
    }
    public MissionData(MissionData MD)
    {
        ownerID = MD.ownerID;
        missionType = MD.missionType;
        progression = MD.progression;
        maxProgression = MD.maxProgression;
    }

    public int ownerID;         // 소유자 ID
    public int missionType;     // 미션 타입
    public int progression;     // 미션 진행도
    public int maxProgression;  // 미션 최대 진행도
}

public class MissionSubSystem : MonoSubSystem
{    
    // 미션 List
    Dictionary<int, MissionData> missions = new Dictionary<int, MissionData>();

    // 마지막으로 생성된 미션의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int missionIDUnique = 0;

    // mission이 달릴 오브젝트 프리팹
    [SerializeField]
    MissionComponent missionPrefeb;
    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        missionIDUnique++;
        return missionIDUnique;
    }

    // mission 생성
    public void Create(int ownerID, int missionType, int progression, int maxProgression)
    {
        int newID = CreateID();
        missions.Add(newID, new MissionData(ownerID, missionType, progression, maxProgression));
        // MissionComponent 생성하여 유저에게 할당하는 코드 필요
        Transform ownerTransform = /*ownerID 유저 Object().*/transform;
        MissionComponent mission = Instantiate(missionPrefeb, ownerTransform);
        mission.SetID(newID);
        mission.AddToPlayerComponent(ownerTransform);
    }
    public void TryCreate(int ownerID, int missionType, int progression, int maxProgression)
    {
        Create(ownerID, missionType, progression, maxProgression);
    }

    // mission 제거
    public void Delete(int ID)
    {
        missions.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }


    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return missions.ContainsKey(ID);
    }

    // Data 확인
    public MissionData LoadData(int ID)
    {
        if (missions.ContainsKey(ID))
        {
            // return missions[ID];
            return new MissionData(missions[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // =================== 기능 =================== //

    // 미션 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        missions[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // 미션 진행도 변경
    public void Progress(int ID, int amount)
    {
        MissionData mission = missions[ID];
        mission.progression += amount;
        if(mission.progression > mission.maxProgression)
        {
            mission.progression = mission.maxProgression;
        }
    }
    public void TryProgress(int ID, int amount)
    {
        Progress(ID, amount);
    }

    // 미션 완료
    public void Complete(int ID)
    {
        missions[ID].progression = missions[ID].maxProgression;
    }
    public void TryComplete(int ID)
    {
        Complete(ID);
    }

    
}
