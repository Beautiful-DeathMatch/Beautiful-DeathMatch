using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusComponent : MonoComponent<StatusSubSystem>
{
    // 해당 Component의 ID
    [SerializeField]
    int ID = 0;
    [SerializeField]
    int initialHP = 0;

    // =================== 내부 호출용도 =================== //


    // 유효성 검사
    int Check()
    {
        if (ID == 0)
        {
            Debug.Log("경고! 해당 Component가 List에 등록되어있지 않습니다. 재등록 시도 중...");
            Register();
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

    // 오브젝트 삭제: 현재 컴포넌트가 달려 있는 Object 삭제 -> Status 는 캐릭터 Object에 직접 할당되므로 삭제 금지
    public void DeleteObject()
    {
        //Destroy(this.gameObject);
        Debug.Log("Status Object 삭제 요청 : 삭제 불가");
    }

    // =================== System 조회 함수 =================== //

    public StatusData LoadData()
    {
        if(Check() >0)
            return System.LoadData(ID);  
        else
            return null;      
    }

    // =================== System 요청 함수 =================== //

    // Status 는 캐릭터에 달려 나오므로 System에 최초 등록이 필요함
    public void Register()
    {
        System.TryRegister(this, 1);
        if(initialHP > 0)
        {
            SetInitialHP(initialHP);
        }
    }

    // Status 소유자 변경
    public void Acquire(int ownerID)
    {
        System.TryAcquire(ID, ownerID);
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
    
    // =================== Awake 함수 (최초 생성 시 시스템 등록 용) =================== //
    void Awake()
    {
        Register();
    }

    // =================== Update 함수 (유효성 체크 용) =================== //

    void Update()
    {
        Check();
    }

}
