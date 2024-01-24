using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MonoComponent<TSystem> : MonoBehaviour where TSystem : MonoSystem
{
    protected TSystem cachedSystem = null;

    protected TSystem System
    {
        get
        {
            if (cachedSystem == null)
            {
                var systems = FindObjectsOfType<TSystem>();
                if (systems == null)
                {
                    Debug.LogError($"{typeof(TSystem)}가 현재 씬에 존재하지 않습니다.");
                    return null;
                }
                else if (systems.Length > 1)
                {
                    Debug.LogError($"{typeof(TSystem)}가 현재 씬에 두 개 이상 존재합니다.");
                    return null;
                }

                cachedSystem = systems[0];
            }

            return cachedSystem;
        }    
	}
}
