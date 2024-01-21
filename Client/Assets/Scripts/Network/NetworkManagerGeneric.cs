using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager<T> : NetworkManager where T : NetworkManager
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                if (singleton == null)
                {
                    Debug.LogError($"네트워크 매니저가 초기화되기 전입니다.");
                    return null;
                }

                instance = singleton as T;
            }

            return instance;
        }
    }
}