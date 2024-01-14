using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug
using System.Text;
using Microsoft.Unity.VisualStudio.Editor; // For Debug

public class WeaponComponent : MonoComponent<WeaponSystem>
{
    // class 무기 Instance: {Instance ID, 소유자 ID, 무기 ID, 최대 장탄수, 현재 장탄수, 장전 안한 남은 탄약수}
    [SerializeField]
    int ID = 0;

    static string _ownerID = "ownerID";
    static string _weaponType = "weaponType";
    static string _maxMagazine = "maxMagazine";
    static string _currentMagazine = "currentMagazine";
    static string _remainedMagazine = "remainedMagazine";

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


    // =================== System 조회 함수 =================== //

    public int LoadOwnerID()
    {
        return System.WeaponData(ID, _ownerID);        
    }
    public int LoadWeaponType()
    {
        return System.WeaponData(ID, _weaponType);        
    }
    public int LoadMaxMagazine()
    {
        return System.WeaponData(ID, _maxMagazine);        
    }
    public int LoadCurrentMagazine()
    {
        return System.WeaponData(ID, _currentMagazine);
    }
    public int LoadRemainedMagazine()
    {
        return System.WeaponData(ID, _remainedMagazine);        
    }

    // =================== System 요청 함수 =================== //

    // 무기 소유자 변경
    public void Acquire(int ownerID)
    {
        System.TryAcquire(ID, ownerID);
    }

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

    // 무기 삭제 -> 시스템에 삭제 요청
    public void Delete()
    {
        System.TryDelete(ID);
    }

    // =================== 기타 함수 =================== //

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

    // =================== Update 함수 (유효하지 않은 오브젝트 삭제 용) =================== //

    void Update()
    {
        if(Check() == -1)
        {
            DeleteObject();
        }

        // =================== DEBUG =================== //

        if(Input.GetMouseButtonDown(0)){
            Shot();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            Reload();
        }
        if(Input.GetKeyDown(KeyCode.X)){
            Delete();
        }

    }

    StringBuilder builder_ = new StringBuilder();
    GUIStyle style_ = new GUIStyle();

    private void OnGUI() {

        builder_.Clear();
        builder_.AppendFormat("[{0}", LoadOwnerID());
        builder_.AppendFormat("{0}] ", LoadWeaponType());
        builder_.AppendFormat("{0} ", LoadCurrentMagazine());
        builder_.AppendFormat("/ {0} ", LoadMaxMagazine());
        builder_.AppendFormat("/ {0} ", LoadRemainedMagazine());

        style_.normal.textColor = new Color(0, 0, 0, 1);
        GUI.Label(
            new Rect(Screen.width * 0.9f, Screen.height * 0.8f, Screen.width * 0.98f, Screen.height * 0.88f)
            , builder_.ToString(), style_);
    }

}
