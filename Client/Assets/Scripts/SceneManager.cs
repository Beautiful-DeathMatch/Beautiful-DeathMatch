using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ENUM_SCENE_TYPE
{
    Title = 0,
    RoomBoard = 1,
    InGame = 2
}

public class SceneManager : Singleton<SceneManager>
{
    public void LoadScene(ENUM_SCENE_TYPE sceneType)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneType.ToString());
    }
}
