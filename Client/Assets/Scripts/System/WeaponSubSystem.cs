using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData
{
    public WeaponData(int _ownerID=0, int _weaponIndex=0, WEAPON_TYPE _weaponType=WEAPON_TYPE.NONE, int _damage=0, int _maxMagazine=1, int _currentMagazine=1, int _remainedMagazine=1)
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

    // Component 리스트
    Dictionary<int, WeaponData> components = new Dictionary<int, WeaponData>();

    // 마지막으로 생성된 무기의 고유 ID, 1 부터 시작
    [SerializeField] // For Debug
    int componentIDUnique = 0;

    // Weapon이 달릴 오브젝트 프리팹
    //[SerializeField]
    //WeaponComponent weaponPrefeb;

    
    // ====================== 공통 ==========================//

    // =================== 생성/삭제 =================== //

    // Component 고유 ID 생성 및 반환
    int CreateID()
    {
        componentIDUnique++;
        return componentIDUnique;
    }

    // Component가 자기 자신의 등록을 요청했을 경우
    public void Register(WeaponComponent component, int ownerID, WeaponData data = null)
    {
        if (data == null)
            TryCreateData(component, new WeaponData(ownerID));
        else
            TryCreateData(component, data);
    }
    public void TryRegister(WeaponComponent component, int ownerID, WeaponData data = null)
    {
        Register(component, ownerID, data);
    }

    // Component Data 생성
    public void CreateData(WeaponComponent component, WeaponData data)
    {
        int newID = CreateID();
        components.Add(newID, new WeaponData(data));
        component.SetID(newID);
    }
    public void TryCreateData(WeaponComponent component, WeaponData data)
    {
        CreateData(component, data);
    }

    // Component 제거
    public void Delete(int ID)
    {
        components.Remove(ID);
    }
    public void TryDelete(int ID)
    {
        Delete(ID);
    }

    // component 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        components[ID].ownerID = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }


    // =================== 외부에서의 Data 확인용 =================== //

    // 존재 여부 확인
    public bool IsContainsKey(int ID)
    {
        return components.ContainsKey(ID);
    }

    // Data 확인
    public WeaponData LoadData(int ID)
    {
        if (components.ContainsKey(ID))
        {
            return new WeaponData(components[ID]); // 보안을 위해?
        }
        else
        {
            Debug.Log("Data 조회 실패!");
            return null;
        }
    }

    // ownerID로 Data 리스트 찾기
    public List<WeaponData> FindByOwnerID(int ownerID)
    {
        List<WeaponData> returnComponents = new();
        foreach (WeaponData component in components.Values)
        {
            if (component.ownerID == ownerID)
                returnComponents.Add(component);
        }
        return returnComponents;
    }

    // ====================== 공통 끝 ==========================//

    // =================== 기능 =================== //


    // 탄약 소모
    public void Shot(int ID)
    {
        WeaponData weapon = components[ID];
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
        WeaponData weapon = components[ID];
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
