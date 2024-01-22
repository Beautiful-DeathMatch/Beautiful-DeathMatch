using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Text;

public class WeaponComponent : MonoComponent<WeaponSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;
    WeaponData initialData = null;
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
    public WeaponData LoadData()
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
    public void Register(WeaponData data = null)
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
        playerComponent.WeaponAdd(this);
    }

    void DeleteFromPlayerComponent() // owner인 Player Component 에 자기 자신 삭제
    {
        try
        {
            playerComponent.WeaponDelete(this);
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

    // 탄약 소모
    public void Shot()
    {
        System.TryShot(ID);
    }

    // 탄약 재장전
    public void Reload()
    {
        System.TryReload(ID);
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
