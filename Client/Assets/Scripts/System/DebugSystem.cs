using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class DebugSystem : MonoSystem   
{
    [SerializeField] WeaponSubSystem weaponSubSystem;
    [SerializeField] StatusSubSystem statusSubSystem;
    [SerializeField] MissionSubSystem missionSubSystem;
    [SerializeField] ItemSubSystem itemSubSystem;
    [SerializeField] TempDB tempDB;
    [SerializeField] BattleMainWindow battleMainWindow;
    [SerializeField] StarterAssetsInputs starterAssetsInputs;

    List<PlayerInfo> players = new();
    int myPlayerId;
    GUIStyle style_ = new GUIStyle();
    public bool isDebugOn { get; private set; } = false;

    // Weapon이 달릴 오브젝트 프리팹
    [SerializeField]
    WeaponComponent weaponPrefeb;

    // mission이 달릴 오브젝트 프리팹
    [SerializeField]
    MissionComponent missionPrefeb;

    // Item이 달릴 오브젝트 프리팹
    [SerializeField]
    ItemComponent itemPrefeb;

    private void CreateWeaponObject(int ownerID, int weaponIndex, int currentMagazine, int remainedMagazine)
    {
        int damage = TempDB.tempWeaponDB[weaponIndex].damage;
        WeaponData.WEAPON_TYPE weaponType = TempDB.tempWeaponDB[weaponIndex].weaponType;
        int maxMagazine = TempDB.tempWeaponDB[weaponIndex].maxMagazine;

        WeaponData newData = new(ownerID, weaponIndex, weaponType, damage, maxMagazine, currentMagazine, remainedMagazine);
        WeaponComponent component = Instantiate(weaponPrefeb, weaponSubSystem.transform);
        component.Register(newData);
        //component.AddToPlayerComponent()
    }

    private void CreateItemObject(int ownerID, int itemIndex)
    {
        int initialMagazine = TempDB.tempItemDB[itemIndex].initialMagazine;

        ItemData newData = new(ownerID, itemIndex, initialMagazine);
        ItemComponent component = Instantiate(itemPrefeb, itemSubSystem.transform);
        component.Register(newData);
    }

    private void CreateMissionObject(int ownerID, int missionType, int progression, int maxProgression)
    {
        MissionData newData = new(ownerID, missionType, progression, maxProgression);
        MissionComponent component = Instantiate(missionPrefeb, missionSubSystem.transform);
        component.Register(newData);
    }


    public override void OnEnter(SceneModuleParam sceneModuleParam)
    {
        base.OnEnter(sceneModuleParam);
        if(sceneModuleParam is BattleSceneModule.Param param)
        {
            players = param.playerInfoList;
            myPlayerId = param.myPlayerId;
        }
    }

    // 디버그 게임 시작
    void DebugGameStart()
    {
        foreach (PlayerInfo player in players)
        {
            //PlayerComponent playerComponent = FindObjectOfType<SpawnSystem>().GetPlayerComponent(playerId);
            CreateWeaponObject(player.playerId, 2, 5, 15);
            CreateWeaponObject(player.playerId, 1, 1, 10);

            CreateItemObject(player.playerId, 1);
            CreateItemObject(player.playerId, 2);

            CreateMissionObject(player.playerId, 1, 0, 1);
            CreateMissionObject(player.playerId, 2, 0, 5);

        }

        battleMainWindow.tempFolder.SetActive(true);
        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;
        isDebugOn = true;
    }

    private void OnGUI() 
    {
        style_.normal.textColor = new Color(0, 0, 0, 1);
        GUI.Label(
            new Rect(Screen.width * 0.85f, Screen.height * 0.9f, Screen.width * 0.98f, Screen.height * 0.98f)
            , "캐릭터 생성 후 Z를 눌러 기본세팅 ", style_);
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Z) && (isDebugOn==false)){
            Debug.Log(players.Count);
            if (players.Count > 0)
            {
                DebugGameStart();
            }
        }
    }

}
