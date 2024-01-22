using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각종 상호작용 오브젝트에 할당되는 Component
// ex) 유저가 바라봄 -> 해당 오브젝트의 정보 제공 ( ex) n번 미션 오브젝트)
// -> 유저의 Player Component가 해당 상호작용의 유효성 판단 (Mission System을 확인)
// -> 유효할 경우 Player 의 UI에 상호작용 UI 출력
// 상호작용 시도 시 -> 해당 오브젝트의 정보 다시 제공
// -> 해당 정보를 바탕으로 Player 의 Mission Component 진행도 변경 (Mission Component의 함수 이용)
// Interaction Component가 가지고 있어야 할 정보:
// ID, InteractionType, SubType,

public class InteractionComponent : MonoComponent<InteractionSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;
    InteractionData initialData = null;
    // InteractionComponent 는 맵에 기본 배치될 경우 초기 데이터 값을 가지고 있음
    [SerializeField]
    InteractionData.INTERACTION_TYPE interactionType;
    [SerializeField]
    int subType;

    // ====================== 공통 ==========================//

    // 유효성 검사
    int Check()
    {
        if (ID == 0)
        {
            Debug.Log("경고! 해당 Component가 List에 등록되어있지 않습니다. 재등록 시도합니다.");
            Register(initialData);
            return 0;
        }
        else if (!System.IsContainsKey(ID))
        {
            Debug.Log("경고! 해당 Component의 ID를 찾을 수 없습니다.");
            return -1;
        }
        else return ID;
    }

    // ID 반환
    public int ReturnID()
    {
        return ID;
    }

    // 시스템에 의한 ID 설정
    public void SetID(int id)
    {
        ID = id;
    }

    // System Data 조회 함수 
    public InteractionData LoadData()
    {
        if(Check() >0)
            return System.LoadData(ID);  
        else
            return null;      
    }

    // 오브젝트 삭제: 현재 컴포넌트가 달려 있는 Object 삭제
    public void DeleteObject()
    {
        Destroy(this.gameObject);
    }

    // =================== System 요청 함수 =================== //

    public void Register(InteractionData data = null)
    {
        initialData = data;
        System.TryRegister(this, 0, data);
    }

    // 소유자 변경
    public void Acquire(int ownerID)
    {
        System.TryAcquire(ID, ownerID);
    }

    // 컴포넌트 삭제 -> 시스템에 삭제 요청
    public void Delete()
    {
        System.TryDelete(ID);
    }
    
    // ====================== 공통 끝 ==========================//

    // =================== System 조회 함수 =================== //

    public InteractionData.INTERACTION_TYPE GetInteractionType()
    {
        InteractionData interaction = LoadData();
        return interaction.interactionType;
    }
    public int GetSubType()
    {
        InteractionData interaction = LoadData();
        return interaction.subType;
    }

    // =================== Start 함수 (Register 용) =================== //

    private void Start() 
    {
        initialData = new(0, interactionType, subType);
        Register(initialData);
    }

    // =================== Update 함수 (유효하지 않은 오브젝트 삭제 용) =================== //

    void Update()
    {
        if (Check() == -1)
        {
            DeleteObject();
        }

        // =================== DEBUG =================== //

    }


}
