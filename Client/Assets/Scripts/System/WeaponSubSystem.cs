using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using UnityEngine;

public class WeaponData
{
    public WeaponData(int _ownerID, int _weaponIndex, WEAPON_TYPE _weaponType, int _damage, int _maxMagazine, int _currentMagazine, int _remainedMagazine)
    {
        ownerID = _ownerID;
        weaponIndex = _weaponIndex;
        weaponType = _weaponType;
        damage = _damage;
        maxMagazine = _maxMagazine;
        currentMagazine = _currentMagazine;
        remainedMagazine = _remainedMagazine;
    }
    public WeaponData(WeaponData WD)
    {
        ownerID = WD.ownerID;
        weaponIndex = WD.weaponIndex;
        weaponType = WD.weaponType;
        damage = WD.damage;
        maxMagazine = WD.maxMagazine;
        currentMagazine = WD.currentMagazine;
        remainedMagazine = WD.remainedMagazine;
    }
    public int ownerID = 0;                                 // 소유자 ID
    public int weaponIndex = 0;                             // 무기 Index
    public WEAPON_TYPE weaponType = WEAPON_TYPE.NONE;       // 무기 타입 (칼/총 등)
    public int damage = 0;                                  // 기본 공격력
    public int maxMagazine = 0;                             // 최대 장전 가능 탄창
    public int currentMagazine = 0;                         // 현재 장전된 탄환 수
    public int remainedMagazine = 0;                        // 장전 안한 남은 탄환 수

    public enum WEAPON_TYPE // 인터랙션 타입 Enum
    {
        NONE,           
        KNIFE,      // 칼
        GUN         // 총
    }
}

public class WeaponSubSystem : MonoSystem
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

    [SerializeField]
    TempDB tempDB;

    

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        weaponIDUnique++;
        return weaponIDUnique;
    }

    // weapon 생성
    public void Create(int ownerID, int weaponIndex, int currentMagazine, int remainedMagazine)
    {
        //int newID = CreateID();
        //WeaponDBData weaponDB = tempDB.GetWeaponDB(weaponIndex); // DB Load
        //weapons.Add(newID, new WeaponData(ownerID, weaponIndex, weaponDB.weaponType, weaponDB.damage, weaponDB.maxMagazine, currentMagazine, remainedMagazine));
        //// WeaponComponent 생성하여 유저에게 할당
        //Transform ownerTransform = FindObjectOfType<SpawnSystem>().GetPlayerComponent(ownerID).transform;
        //WeaponComponent weapon = Instantiate(weaponPrefeb, ownerTransform);
        //weapon.SetID(newID);
        //weapon.AddToPlayerComponent(ownerTransform);
        
    }
    public void TryCreate(int ownerID, int weaponIndex, int currentMagazine, int remainedMagazine)
    {
        Create(ownerID, weaponIndex, currentMagazine, remainedMagazine);
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
        if (weapon.weaponType != WeaponData.WEAPON_TYPE.KNIFE && weapon.currentMagazine > 0)
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

}
