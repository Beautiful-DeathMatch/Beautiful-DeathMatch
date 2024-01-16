using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionComponent : MonoComponent<MissionSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;

    // =================== 내부 호출용도 =================== //

    // 무기 정보 로드 from json DB
    // DBLoad()


    // 유효성 검사
    int Check()
    {
        if (ID == 0)
        {
            Debug.Log("경고! 해당 Component가 List에 등록되어있지 않습니다.");
            return 0;
        }
        else if (!System.IsContainsKey(ID))
        {
            Debug.Log("경고! 해당 Component의 ID를 찾을 수 없습니다.");
            return -1;
        }
        else return ID;
    }

    // =================== 외부 반환 용도 =================== //

    // ID 반환
    public int ReturnID()
    {
        return ID;
    }

    // =================== System 에 의한 호출 함수 =================== //

    // 시스템에 의한 ID 설정
    public void SetID(int id)
    {
        ID = id;
    }

    // 오브젝트 삭제: 현재 컴포넌트가 달려 있는 Object 삭제
    public void DeleteObject()
    {
        Destroy(this.gameObject);
    }

    // =================== System 조회 함수 =================== //

    public MissionData LoadData()
    {
        if(Check() >0)
            return System.LoadData(ID);  
        else
            return null;      
    }

    // =================== System 요청 함수 =================== //

    public void Acquire(int ID, int ownerID)
    {
        System.TryAcquire(ID, ownerID);
    }

    // 미션 진행도 변경
    public void Progress(int ID, int amount)
    {
        System.TryProgress(ID, amount);
    }

    // 미션 완료
    public void Complete(int ID)
    {
        System.Complete(ID);
    }

}
