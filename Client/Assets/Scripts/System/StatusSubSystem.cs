using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusData
{
    public StatusData(int _ownerID, int _maxHp, int _hp, STATE _state, PHASE _phase)
    {
        ownerID = _ownerID;
        maxHp = _maxHp;    
        hp = _hp;          
        state = _state;    
        phase = _phase;    
    }
    public StatusData(StatusData SD)
    {
        ownerID = SD.ownerID;
        maxHp = SD.maxHp;    
        hp = SD.hp;          
        state = SD.state;    
        phase = SD.phase;    
    }

    public int ownerID;                     // 소유자 ID
    public int maxHp = 100;                 // 최대 체력
    public int hp = 100;                    // 현재 체력
    public STATE state = STATE.UNKNOWN;     // 상태 (생존/사망/부활대기 등)
    public PHASE phase = PHASE.UNKNOWN;     // 도달 구간 (미션완료/헬기탑승 등)

    public enum STATE // 캐릭터의 현재 상태 Enum
    {
        UNKNOWN,
        LIVE,
        DEAD,
        REVIVE,
        GOD
    }
    public enum PHASE // 캐릭터의 현재 진행도 Enum
    {
        UNKNOWN,
        READY,
        START,
        MISSION_COMPLETE,
        HELICOPTER,
        END
    }

}

public class StatusSubSystem : MonoSubSystem
{

    // Status 리스트
    Dictionary<int, StatusData> statuses = new Dictionary<int, StatusData>();

    // 마지막으로 생성된 고유 ID
    [SerializeField] // For Debug
    int statusIDUnique = 0;

    // status Component는 프리팹 미사용
    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        statusIDUnique++;
        return statusIDUnique;
    }

    // status 생성
    public void Create(StatusComponent statusComponent, int ownerID, int maxHP, int hp, StatusData.STATE state, StatusData.PHASE phase)
    {
        int newID = CreateID();
        statuses.Add(newID, new StatusData(ownerID, maxHP, hp, state, phase));
        statusComponent.SetID(newID);

    }
    public void TryCreate(StatusComponent statusComponent, int ownerID, int maxHP, int hp, StatusData.STATE state, StatusData.PHASE phase)
    {
        Create(statusComponent, ownerID, maxHP, hp, state, phase);
    }

    // weapon 제거
    public void Delete(int ID)
    {
        statuses.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }

    // Component가 자기 자신의 등록을 요청했을 경우
    public void Register(StatusComponent statusComponent, int ownerID)
    {
        TryCreate(statusComponent, ownerID, 100, 100, StatusData.STATE.UNKNOWN, StatusData.PHASE.UNKNOWN);
    }
    public void TryRegister(StatusComponent statusComponent, int ownerID)
    {
        Register(statusComponent, ownerID);
    }

    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return statuses.ContainsKey(ID);
    }

    // Data 확인
    public StatusData LoadData(int ID)
    {
        if (statuses.ContainsKey(ID))
        {
            // return statuses[ID];
            return new StatusData(statuses[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // =================== 기능 =================== //

    // Status 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        statuses[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // 피격
    public void Hit(int ID, int amount)
    {
        StatusData status = statuses[ID];
        status.hp -= amount;
        if (status.hp <= 0)
        {
            status.hp = 0;
            Dead(ID);
        }
    }
    public void TryHit(int ID, int amount)
    {
        Hit(ID, amount);
    }

    // 힐
    public void Heal(int ID, int amount)
    {
        StatusData status = statuses[ID];
        status.hp += amount;
        if (status.hp > status.maxHp)
            status.hp = status.maxHp;
    }
    public void TryHeal(int ID, int amount)
    {
        Heal(ID, amount);
    }    

    // 죽음 처리 (시스템에 의해)
    public void Dead(int ID)
    {
        statuses[ID].state = StatusData.STATE.DEAD;
    }
    public void TryDead(int ID)
    {
        Dead(ID);
    }

    // 게임 준비
    public void AllGameReady()
    {
        foreach (int ID in statuses.Keys)
        {
            statuses[ID].phase = StatusData.PHASE.READY;
        }
    }
    public void TryAllGameReady()
    {
        AllGameReady();
    }

    // 게임 시작
    public void AllGameStart()
    {
        foreach (int ID in statuses.Keys)
        {
            statuses[ID].phase = StatusData.PHASE.START;
        }
    }
    public void TryAllGameStart()
    {
        AllGameStart();
    }

    // 초기 MaxHP 세팅 (Component에 설정 되어 있을 경우에만)
    public void SetInitialHP(int ID, int amount)
    {
        statuses[ID].maxHp = amount;
        statuses[ID].hp = statuses[ID].maxHp;
    }
    public void TrySetInitialHP(int ID, int amount)
    {
        SetInitialHP(ID, amount);
    }

}
