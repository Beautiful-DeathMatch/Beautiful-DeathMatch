using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusComponent : MonoComponent<StatusSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;
    StatusData initialData = null;
    [SerializeField]
    int initialHP = 0;

    // =================== 내부 호출용도 =================== //


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
    public StatusData LoadData()
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
    public void Register(StatusData data = null)
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

    // =================== 기능 =================== //

    public void InitialSet()
    {
        if(initialHP > 0)
        {
            SetInitialHP(initialHP);
        }
    }

    // 피격
    public void Hit(int amount)
    {
        System.TryHit(ID, amount);
    }

    // 힐
    public void Heal(int amount)
    {
        System.TryHeal(ID, amount);
    }    

    // 죽음
    public void Dead(int ID)
    {
        System.TryDead(ID);
    }

    // 초기 MaxHP 세팅 (Component에 설정 되어 있을 경우에만)
    public void SetInitialHP(int amount)
    {
        System.SetInitialHP(ID, amount);
    }
    
    // =================== Start 함수 (Register 용) =================== //
    void Start()
    {
        if (initialHP > 0)
        {
            StatusData initialData = new(0, initialHP, initialHP);
            Register(initialData);
        }
        else
            Register();
    }

    // =================== Update 함수 (유효성 체크 용) =================== //

    void Update()
    {
        Check();
    }

}
