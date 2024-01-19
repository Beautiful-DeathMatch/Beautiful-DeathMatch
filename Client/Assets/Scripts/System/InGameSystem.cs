using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class InGameSystem : MonoSystem
{

    WeaponSubSystem weaponSubSystem;
    StatusSubSystem statusSubSystem;
    MissionSubSystem missionSubSystem;
    public List<int> playerIdList= new();
    public bool isDebugOn { get; private set; } = false;

    TempDB tempDB;

    private void Awake() 
    {
        statusSubSystem = FindObjectOfType<StatusSubSystem>();
        weaponSubSystem = FindObjectOfType<WeaponSubSystem>();
        missionSubSystem = FindObjectOfType<MissionSubSystem>();
        tempDB = FindObjectOfType<TempDB>();
    }

    // 게임 준비
    void GameReady()
    {
        statusSubSystem.AllGameReady();
    }

    // 게임 시작
    void GameStart()
    {
        statusSubSystem.AllGameStart();
    }

    // 디버그 게임 시작
    void DebugGameStart()
    {
        foreach (int playerId in playerIdList)
        {
            //PlayerComponent playerComponent = FindObjectOfType<SpawnSystem>().GetPlayerComponent(playerId);
            weaponSubSystem.TryCreate(playerId, 2, 5, 15);
            weaponSubSystem.TryCreate(playerId, 1, 1, 10);

            missionSubSystem.TryCreate(playerId, 1, 0, 1);
            missionSubSystem.TryCreate(playerId, 2, 0, 5);
        }
        FindObjectOfType<UISystem>().canvas.SetActive(true);
        FindObjectOfType<StarterAssetsInputs>().cursorLocked = true;
        FindObjectOfType<StarterAssetsInputs>().cursorInputForLook = true;
        isDebugOn = true;
    }

    // 신규 플레이어 입장 시 해야 할 것
    public void PlayerSet(int PlayerID, PlayerComponent playerComponent)
    {
        // Status 등록 -> Status Component가 알아서 함
        // Status 의 초기값을 알맞게 조정 from DB
        // 기본 Mission 제공 from DB
        // 기본 Weapon 제공 from DB
    }

    // ==============DEBUG============= //

    GUIStyle style_ = new GUIStyle();

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
            if (playerIdList.Count > 0)
            {
                DebugGameStart();
            }
        }
    }

}
