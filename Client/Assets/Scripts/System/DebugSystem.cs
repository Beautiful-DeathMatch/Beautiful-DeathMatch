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

    [SerializeField] StarterAssetsInputs starterAssetsInputs;

    private List<PlayerInfo> players = new List<PlayerInfo>();
	private bool isDebugOn = false;

    // Weapon이 달릴 오브젝트 프리팹
    [SerializeField]
    FieldWeaponComponent weaponPrefeb;

    // mission이 달릴 오브젝트 프리팹
    [SerializeField]
    MissionComponent missionPrefeb;

    // Item이 달릴 오브젝트 프리팹
    [SerializeField]
    FieldItemComponent itemPrefeb;

    private void CreateWeaponObject(int ownerID, int weaponIndex, int currentMagazine, int remainedMagazine)
    {
        int damage = TempDB.tempWeaponDB[weaponIndex].damage;
        WeaponData.WEAPON_TYPE weaponType = TempDB.tempWeaponDB[weaponIndex].weaponType;
        int maxMagazine = TempDB.tempWeaponDB[weaponIndex].maxMagazine;

        WeaponData newData = new(ownerID, weaponIndex, weaponType, damage, maxMagazine, currentMagazine, remainedMagazine);
        FieldWeaponComponent component = Instantiate(weaponPrefeb, weaponSubSystem.transform);
        component.Register(newData);
    }

    private void CreateItemObject(int ownerID, int itemIndex)
    {
        int initialMagazine = TempDB.tempItemDB[itemIndex].initialMagazine;

        ItemData newData = new(ownerID, itemIndex, initialMagazine);
        FieldItemComponent component = Instantiate(itemPrefeb, itemSubSystem.transform);
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

        if (sceneModuleParam is BattleSceneModule.Param param)
        {
            players = param.playerInfoList;
        }
    }

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width * 0.85f, Screen.height * 0.9f, Screen.width * 0.98f, Screen.height * 0.98f), "디버그 모드 시작"))
		{
			if (players.Count > 0 && !isDebugOn)
			{
				DebugGameStart();
				isDebugOn = true;
			}
		}
	}

	// 디버그 게임 시작
	private void DebugGameStart()
    {
        foreach (PlayerInfo player in players)
        {
            CreateWeaponObject(player.playerId, 2, 5, 15);
            CreateWeaponObject(player.playerId, 1, 1, 10);

            CreateItemObject(player.playerId, 1);
            CreateItemObject(player.playerId, 2);

            CreateMissionObject(player.playerId, 1, 0, 1);
            CreateMissionObject(player.playerId, 2, 0, 5);
        }

        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;
    }
}
