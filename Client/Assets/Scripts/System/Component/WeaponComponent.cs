using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug
using System.Text;
using Microsoft.Unity.VisualStudio.Editor; // For Debug

public class WeaponComponent : MonoComponent<WeaponSubSystem>
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

    public WeaponData LoadData()
    {
        if(Check() >0)
            return System.LoadData(ID);  
        else
            return null;      
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

        if(Check() == -1)
        {
            DeleteObject();
        }

        builder_.Clear();
        builder_.AppendFormat("[{0}", LoadData().ownerID);
        builder_.AppendFormat("{0}] ", LoadData().weaponType);
        builder_.AppendFormat("{0} ", LoadData().currentMagazine);
        builder_.AppendFormat("/ {0} ", LoadData().maxMagazine);
        builder_.AppendFormat("/ {0} ", LoadData().remainedMagazine);

        style_.normal.textColor = new Color(0, 0, 0, 1);
        GUI.Label(
            new Rect(Screen.width * 0.9f, Screen.height * 0.8f, Screen.width * 0.98f, Screen.height * 0.88f)
            , builder_.ToString(), style_);
    }

}
