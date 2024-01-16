using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSystem : MonoSystem
{

    WeaponSubSystem weaponSubSystem;
    StatusSubSystem statusSubSystem;
    MissionSubSystem missionSubSystem;

    private void Awake() 
    {
        statusSubSystem = FindObjectOfType<StatusSubSystem>();
        weaponSubSystem = FindObjectOfType<WeaponSubSystem>();
        missionSubSystem = FindObjectOfType<MissionSubSystem>();
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

    // 신규 플레이어 입장 시 해야 할 것
    public void PlayerSet(int PlayerID, PlayerComponent playerComponent)
    {
        // Status 등록 -> Status Component가 알아서 함
        // Status 의 초기값을 알맞게 조정 from DB
        // 기본 Mission 제공 from DB
        // 기본 Weapon 제공 from DB
    }

    //

}
