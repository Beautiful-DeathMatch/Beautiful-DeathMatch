using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponComponent : MonoComponent<WeaponSystem>
{
    // class 무기 Instance: {Instance ID, 소유자 ID, 무기 ID, 최대 장탄수, 현재 장탄수, 장전 안한 남은 탄약수}
    int _ID = 0;
    int _ownerID;
    int _weaponID;
    int _maxMagazine;
    int _currentMagazine;
    int _remainedMagazine;

    // 무기 상태 변경 (디버깅용)
    void Change(int ID, int ownerID, int weaponID, int maxMagazine, int currentMagazine, int remainedMagazine)
    {
        _ID = ID;
        _ownerID = ownerID;
        _weaponID = weaponID;
        _maxMagazine = maxMagazine;
        _currentMagazine = currentMagazine;
        _remainedMagazine = remainedMagazine;
    }

    // =================== 내부 호출용도 =================== //

    // 무기 정보 로드 from json DB
    // DBLoad()

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

    // 무기 소유자 변경
    public void Acquire(int ownerID, bool bySystem = false)
    {
        _ownerID = ownerID; 
    }

    // 무기 버리기 (기획 확인 필요)
    // Throw(Instance ID)

    // 탄약 소모
    public void Shot(int shotNum = 1)
    {
        _currentMagazine -= shotNum;
    }

    // 탄약 재장전
    public void ReLoad()
    {
        int emptyMegazine = _maxMagazine - _currentMagazine;
        if(emptyMegazine <= _remainedMagazine)
        {
            _currentMagazine += emptyMegazine;
            _remainedMagazine -= emptyMegazine;
        } 
        else
        {
            _currentMagazine += _remainedMagazine;
            _remainedMagazine -= _remainedMagazine;
        }
    }

    // 무기 삭제 -> 시스템에 삭제 요청
    public void Delete()
    {
        System.Delete(this);
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


    private void Awake() 
    {
        // 자동 등록 후 유효성 검사
        if (_ID == 0)
            Register();
        Check();
    }


}
