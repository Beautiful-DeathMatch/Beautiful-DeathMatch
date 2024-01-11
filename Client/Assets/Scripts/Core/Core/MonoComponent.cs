using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MonoComponent<TSystem> : MonoBehaviour where TSystem : MonoSubSystem
{
    protected TSystem FindSystem()
    {
        var systems = FindObjectsOfType<TSystem>();
        if (systems == null)
        {
            Debug.LogError($"{typeof(TSystem)}�� ���� ���� �������� �ʽ��ϴ�.");
            return null;
        }
		else if (systems.Length > 1)
		{
			Debug.LogError($"{typeof(TSystem)}�� ���� ���� �� �� �̻� �����մϴ�.");
			return null;
		}

        return systems[0];
	}
}
