using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData
{
    public InteractionData(int _ownerID=0, INTERACTION_TYPE _interactionType=INTERACTION_TYPE.NONE, int _subType=0)
    {
        ownerID = _ownerID;
        interactionType = _interactionType;
        subType = _subType;
    }
    public InteractionData(InteractionData _ID)
    {
        ownerID = _ID.ownerID;
        interactionType = _ID.interactionType;
        subType = _ID.subType;
    }

    public int ownerID;                     // 소유자 ID (Interaction에선 미사용 예정, Mono 호환용)
    public INTERACTION_TYPE interactionType;     // 인터랙션 타입   
    public int subType;                     // 세부 타입

    public enum INTERACTION_TYPE // 인터랙션 타입 Enum
    {
        NONE,           
        MISSION,        // 미션
        BOX,            // 보물상자
        CALL,           // 헬리콥터 호출기
        HELICOPTER      // 헬리콥터 탑승
    }

}

public class InteractionSubSystem : MonoSystem
{
    // List
    Dictionary<int, InteractionData> components = new Dictionary<int, InteractionData>();

    // 마지막으로 생성된 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int componentIDUnique = 0;
    
    // ====================== 공통 ==========================//

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        componentIDUnique++;
        return componentIDUnique;
    }

    // Component가 자기 자신의 등록을 요청했을 경우
    public void Register(InteractionComponent component, int ownerID, InteractionData data = null)
    {
        if (data == null)
            TryCreateData(component, new InteractionData(ownerID));
        else
            TryCreateData(component, data);
    }
    public void TryRegister(InteractionComponent component, int ownerID, InteractionData data = null)
    {
        Register(component, ownerID, data);
    }

    // Component Data 생성
    public void CreateData(InteractionComponent component, InteractionData data)
    {
        int newID = CreateID();
        components.Add(newID, new InteractionData(data));
        component.SetID(newID);
    }
    public void TryCreateData(InteractionComponent component, InteractionData data)
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
    public InteractionData LoadData(int ID)
    {
        if (components.ContainsKey(ID))
        {
            return new InteractionData(components[ID]);
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // ownerID로 Data 리스트 찾기
    public List<InteractionData> FindByOwnerID(int ownerID)
    {
        List<InteractionData> returnComponents = new();
        foreach (InteractionData component in components.Values)
        {
            if (component.ownerID == ownerID)
                returnComponents.Add(component);
        }
        return returnComponents;
    }

    // ====================== 공통 끝 ==========================//

    // =================== 기능 =================== //


}
