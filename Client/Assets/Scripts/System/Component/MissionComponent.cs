using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionComponent : MonoComponent<MissionSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;
    MissionData initialData = null;
    // 해당 Component의 PlayerComponent
    [SerializeField]
    PlayerComponent playerComponent = null;

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
    public MissionData LoadData()
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

    // System에 최초 등록 요청
    public void Register(MissionData data = null)
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

    // =================== Player Component 관련 =================== //
    

    // owner인 Player Component 에 자기 자신 추가
    public void AddToPlayerComponent(Transform transform)
    {
        playerComponent = transform.GetComponent<PlayerComponent>();
        playerComponent.MissionAdd(this);
    }

    void DeleteFromPlayerComponent() // owner인 Player Component 에 자기 자신 삭제
    {
        try
        {
            playerComponent.MissionDelete(this);
        }
        catch(Exception e)
        {
            Debug.Log("Player Component의 리스트에서 삭제 실패 : "+ e);
        }
    }

    // 오브젝트 삭제: 현재 컴포넌트가 달려 있는 Object 삭제
    public void DeleteObjectAtPlayer()
    {
        DeleteFromPlayerComponent();
        Destroy(this.gameObject);
    }

    // =================== 기능 =================== //

    // 미션 유효 여부 (진행 가능 여부) 조회
    public bool IsMissionInProgress()
    {
        if (Check() <= 0)
            return false;
        MissionData mission = System.LoadData(ID);
        if (mission.progression < mission.maxProgression)
            return true;
        return false;
    }

    // 미션 진행도 변경
    public void Progress(int amount)
    {
        System.TryProgress(ID, amount);
    }

    // 미션 완료
    public void Complete()
    {
        System.TryComplete(ID);
    }

}
