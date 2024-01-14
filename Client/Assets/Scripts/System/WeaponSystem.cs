using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class WeaponSystem : MonoSystem
{

    // 무기 List
    // List<WeaponComponent> weapons = new List<WeaponComponent>();
    Dictionary<int, Dictionary<string, int>> weapons = new Dictionary<int, Dictionary<string, int>>();

    string _ownerID = "ownerID";
    string _weaponType = "weaponType";
    string _maxMagazine = "maxMagazine";
    string _currentMagazine = "currentMagazine";
    string _remainedMagazine = "remainedMagazine";


    // 마지막으로 생성된 무기의 고유 ID, 1 부터 시작
    static int weaponIDUnique = 0;

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
        weapons.Add(newID, new Dictionary<string, int>(){
        {_ownerID, ownerID},
        {_weaponType, weaponType},
        {_maxMagazine, maxMagazine},
        {_currentMagazine, currentMagazine},
        {_remainedMagazine, remainedMagazine}
        });
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
    public int WeaponData(int ID, string dataName)
    {
        if (weapons.ContainsKey(ID))
            return weapons[ID][dataName];
        else
        {
            Debug.Log("Data 조회 실패!");
            return -1;
        }
    }

    // =================== 기능 =================== //

    // 무기 소유자 변경
    public void Acquire(int ID, int ownerID)
    {
        weapons[ID][_ownerID] = ownerID;
    }
    public void TryAcquire(int ID, int ownerID)
    {
        Acquire(ID, ownerID);
    }

    // 탄약 소모
    public void Shot(int ID)
    {
        int currentMagazine = weapons[ID][_currentMagazine];
        if (currentMagazine > 0)
            weapons[ID][_currentMagazine] -= 1;
    }
    public void TryShot(int ID)
    {
        Shot(ID);
    }

    // 탄약 재장전
    public void Reload(int ID)
    {
        int currentMagazine = weapons[ID][_currentMagazine],
            maxMagazine = weapons[ID][_maxMagazine],
            remainedMagazine = weapons[ID][_remainedMagazine];
        int consumedMegazine = maxMagazine - currentMagazine;

        if (consumedMegazine <= remainedMagazine)
        {
            currentMagazine += consumedMegazine;
            remainedMagazine -= consumedMegazine;
        }
        else
        {
            currentMagazine += remainedMagazine;
            remainedMagazine -= remainedMagazine;
        }
        weapons[ID][_currentMagazine] = currentMagazine;
        weapons[ID][_maxMagazine] = maxMagazine;
        weapons[ID][_remainedMagazine] = remainedMagazine;
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
