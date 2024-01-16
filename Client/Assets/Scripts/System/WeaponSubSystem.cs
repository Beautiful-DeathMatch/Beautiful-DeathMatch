using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class WeaponData
{
    public WeaponData(int _ownerID, int _weaponType, int _maxMagazine, int _currentMagazine, int _remainedMagazine)
    {
        ownerID = _ownerID;
        weaponType = _weaponType;
        maxMagazine = _maxMagazine;
        currentMagazine = _currentMagazine;
        remainedMagazine = _remainedMagazine;
    }
    public WeaponData(WeaponData WD)
    {
        ownerID = WD.ownerID;
        weaponType = WD.weaponType;
        maxMagazine = WD.maxMagazine;
        currentMagazine = WD.currentMagazine;
        remainedMagazine = WD.remainedMagazine;
    }
    public int ownerID = 0;             // 소유자 ID
    public int weaponType = 0;          // 무기 타입 (칼/총 등)
    public int maxMagazine = 0;         // 최대 장전 가능 탄창
    public int currentMagazine = 0;     // 현재 장전된 탄환 수
    public int remainedMagazine = 0;    // 장전 안한 남은 탄환 수
}

public class WeaponSubSystem : MonoSubSystem
{

    // 무기 List
    // List<WeaponComponent> weapons = new List<WeaponComponent>();
    Dictionary<int, WeaponData> weapons = new Dictionary<int, WeaponData>();


    // 마지막으로 생성된 무기의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int weaponIDUnique = 0;

    // Weapon이 달릴 오브젝트 프리팹
    [SerializeField]
    WeaponComponent weaponPrefeb;
    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        weaponIDUnique++;
        return weaponIDUnique;
    }

    // weapon 생성
    public void Create(int ownerID, int weaponType, int maxMagazine, int currentMagazine, int remainedMagazine)
    {
        int newID = CreateID();
        weapons.Add(newID, new WeaponData(ownerID, weaponType, maxMagazine, currentMagazine, remainedMagazine));
        // WeaponComponent 생성하여 유저에게 할당하는 코드 필요
        Instantiate(weaponPrefeb, /*ownerID 유저 Object.*/transform).SetID(newID);

    }
    public void TryCreate(int ownerID, int weaponType, int maxMagazine, int currentMagazine, int remainedMagazine)
    {
        Create(ownerID, weaponType, maxMagazine, currentMagazine, remainedMagazine);
    }

    // weapon 제거
    public void Delete(int ID)
    {
        weapons.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }


    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return weapons.ContainsKey(ID);
    }

    // Data 확인
    public WeaponData LoadData(int ID)
    {
        if (weapons.ContainsKey(ID))
        {
            // return weapons[ID];
            return new WeaponData(weapons[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // =================== 기능 =================== //

    // 무기 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        weapons[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // 탄약 소모
    public void Shot(int ID)
    {
        WeaponData weapon = weapons[ID];
        if (weapon.currentMagazine > 0)
            weapon.currentMagazine -= 1;
    }
    public void TryShot(int ID)
    {
        Shot(ID);
    }

    // 탄약 재장전
    public void Reload(int ID)
    {
        WeaponData weapon = weapons[ID];
        int consumedMegazine = weapon.maxMagazine - weapon.currentMagazine;

        if (consumedMegazine <= weapon.remainedMagazine)
        {
            weapon.currentMagazine += consumedMegazine;
            weapon.remainedMagazine -= consumedMegazine;
        }
        else
        {
            weapon.currentMagazine += weapon.remainedMagazine;
            weapon.remainedMagazine -= weapon.remainedMagazine;
        }
    }
    public void TryReload(int ID)
    {
        Reload(ID);
    }


    // ==============DEBUG============= //

    GUIStyle style_ = new GUIStyle();

    private void OnGUI() {
        style_.normal.textColor = new Color(0, 0, 0, 1);
        GUI.Label(
            new Rect(Screen.width * 0.85f, Screen.height * 0.9f, Screen.width * 0.98f, Screen.height * 0.98f)
            , "Z:Add X:Delete R:Reload " +weapons.Count.ToString(), style_);
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Z)){
            TryCreate(1, 1, 5, 5, 15);
        }
    }
}
