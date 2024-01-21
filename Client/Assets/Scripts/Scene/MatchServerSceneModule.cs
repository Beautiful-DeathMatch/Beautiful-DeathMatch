using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchServerSceneModule : MonoBehaviour
{
    void Start()
    {
        MatchSessionManager.Instance.StartServer();
    }
}
