using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData
{
    public MissionData(int _ownerID=0, int _missionType=0, int _progression=0, int __maxProgression=0)
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

public class MissionSubSystem : MonoSystem
{    
    // 미션 List
    Dictionary<int, MissionData> components = new Dictionary<int, MissionData>();

    // 마지막으로 생성된 미션의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int componentIDUnique = 0;

    // // mission이 달릴 오브젝트 프리팹
    // [SerializeField]
    // MissionComponent missionPrefeb;
    

    // ====================== 공통 ==========================//

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        componentIDUnique++;
        return componentIDUnique;
    }

    // Component가 자기 자신의 등록을 요청했을 경우
    public void Register(MissionComponent component, int ownerID, MissionData data = null)
    {
        if (data == null)
            TryCreateData(component, new MissionData(ownerID));
        else
            TryCreateData(component, data);
    }
    public void TryRegister(MissionComponent component, int ownerID, MissionData data = null)
    {
        Register(component, ownerID, data);
    }

    // Component Data 생성
    public void CreateData(MissionComponent component, MissionData data)
    {
        int newID = CreateID();
        components.Add(newID, new MissionData(data));
        component.SetID(newID);
    }
    public void TryCreateData(MissionComponent component, MissionData data)
    {
        CreateData(component, data);
    }

    // Component 제거
    public void Delete(int ID)
    {
        components.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }

    // component 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        components[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return components.ContainsKey(ID);
    }

    // Data 확인
    public MissionData LoadData(int ID)
    {
        if (components.ContainsKey(ID))
        {
            return new MissionData(components[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // ownerID로 Data 리스트 찾기
    public List<MissionData> FindByOwnerID(int ownerID)
    {
        List<MissionData> returnComponents = new();
        foreach (MissionData component in components.Values)
        {
            if (component.ownerID == ownerID)
                returnComponents.Add(component);
        }
        return returnComponents;
    }

    // ====================== 공통 끝 ==========================//

    // =================== 기능 =================== //

    // 미션 진행도 변경
    public void Progress(int ID, int amount)
    {
        MissionData mission = components[ID];
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
        components[ID].progression = components[ID].maxProgression;
    }
    public void TryComplete(int ID)
    {
        Complete(ID);
    }

    
}
