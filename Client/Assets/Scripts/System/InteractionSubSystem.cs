using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData
{
    public InteractionData(INTERACTION_TYPE _interactionType, int _subType)
    {
        interactionType = _interactionType;
        subType = _subType;
    }
    public InteractionData(InteractionData _ID)
    {
        interactionType = _ID.interactionType;
        subType = _ID.subType;
    }

    public INTERACTION_TYPE interactionType;     // 인터랙션 타입   
    public int subType;                     // 세부 타입

    public enum INTERACTION_TYPE // 인터랙션 타입 Enum
    {
        MISSION,        // 미션
        BOX,            // 보물상자
        CALL,           // 헬리콥터 호출기
        HELICOPTER      // 헬리콥터 탑승
    }

}

public class InteractionSubSystem : MonoSubSystem
{
    // List
    Dictionary<int, InteractionData> interactions = new Dictionary<int, InteractionData>();


    // 마지막으로 생성된 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int interactionIDUnique = 0;

    // Interaction이 달릴 오브젝트 프리팹
    // [SerializeField]
    // InteractionComponent interactionPrefeb;
    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        interactionIDUnique++;
        return interactionIDUnique;
    }

    // interaction 생성
    public void Create(InteractionData.INTERACTION_TYPE interactionType, int subType)
    {
        int newID = CreateID();
        interactions.Add(newID, new InteractionData(interactionType, subType));
        //Instantiate(interactionPrefeb, /*ownerID 유저 Object.*/transform).SetID(newID);

    }
    public void TryCreate(InteractionData.INTERACTION_TYPE interactionType, int subType)
    {
        Create(interactionType, subType);
    }

    // interaction 제거
    public void Delete(int ID)
    {
        interactions.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }


    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return interactions.ContainsKey(ID);
    }

    // Data 확인
    public InteractionData LoadData(int ID)
    {
        if (interactions.ContainsKey(ID))
        {
            return new InteractionData(interactions[ID]);
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // =================== 기능 =================== //
}
