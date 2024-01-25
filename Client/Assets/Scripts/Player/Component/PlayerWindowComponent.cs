using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWindowComponent : MonoComponent<PrefabLinkedUISystem>
{
    private BattleMainWindow battleWindow = null;

	private void Start()
	{
		battleWindow = System.GetMainWindow<BattleMainWindow>(); 
	}
}
