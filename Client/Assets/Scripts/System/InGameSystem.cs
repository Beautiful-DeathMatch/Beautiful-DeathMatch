using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSystem : MonoSystem
{

    StatusSubSystem statusSubSystem;

    private void Awake() 
    {
        statusSubSystem = FindObjectOfType<StatusSubSystem>();
    }

    // 게임 준비
    void GameReady()
    {
        statusSubSystem.GameReady();
    }

    // 게임 시작
    void GameStart()
    {
        statusSubSystem.GameStart();
    }

    //

}
