using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusComponent : MonoComponent<StatusSubSystem>
{
    // class 캐릭터 Instance : {Instance ID, 소유자 ID, hp, state, Phase} // state: 생존/사망/부활대기 등 Phase: 미션완료/헬기탑승 등
    // 각 캐릭터에 할당 될 Component
    [SerializeField]
    int _ID;
    [SerializeField]
    int _ownerID;
    [SerializeField]
    int _maxHp = 100;
    [SerializeField]
    int _hp = 100;
    [SerializeField]
    int _state = 0;
    [SerializeField]
    int _phase = 0;
    public enum STATE // 캐릭터의 현재 상태에 관련
    {
        UNKNOWN,
        LIVE,
        DEAD,
        REVIVE,
        GOD
    }
    public enum PHASE // 캐릭터의 현재 진행도에 관련
    {
        UNKNOWN,
        READY,
        START,
        MISSION_COMPLETE,
        HELICOPTER,
        END
    }
    // =================== 내부 호출용도 =================== //

    // 이 컴포넌트를 시스템 리스트에 등록
    void Register()
    {
        System.ListUp(this);
    }

    // 유효성 검사
    int Check(){
        int index = System.FindListIndexByID(_ID);
        if (index == -1)
        {
            Debug.Log("경고! 해당 Component가 List에 등록되어있지 않습니다.");
            return -1;
        }
        else return index;
    }

    // =================== 외부 반환 용도 =================== //

    // ID 반환
    public int returnID()
    {
        return _ID;
    }


    // =================== Data 변경 =================== //

    // Status 소유자 변경
    public void Acquire(int ownerID, bool bySystem = false)
    {
        _ownerID = ownerID; 
    }

    // 피격
    public int Hit(int amount)
    {
        _hp -= amount;
        if (_hp < 0)
            _hp = 0;
        return _hp;
    }

    // 힐
    public int Heal(int amount)
    {
        _hp += amount;
        if (_hp > _maxHp)
            _hp = _maxHp;
        return _hp;
    }

    // 죽음 처리 (시스템에 의해)
    public void Dead(){
        StateChange((int)STATE.DEAD);
    }

    public void GameReady()
    {
        PhaseChange((int)PHASE.READY);
    }

    public void GameStart()
    {
        PhaseChange((int)PHASE.START);
    }

    // state 변경
    void StateChange(int newState)
    {
        _state = newState;
    }

    // phase 변경
    void PhaseChange(int newPhase)
    {
        _phase = newPhase;
    }



    // 시스템에 의한 삭제: 현재 컴포넌트가 달려 있는 Object 삭제
    public void DeleteBySystem()
    {
        Destroy(this.gameObject);
    }

    // 시스템에 의한 ID 세팅
    public void SetID(int ID){
        _ID = ID;
    }


    // =================== Awake =================== //


    void Awake() 
    {
        // 자동 등록 후 유효성 검사
        if (_ID == 0)
            Register();
        Check();
    }

}
