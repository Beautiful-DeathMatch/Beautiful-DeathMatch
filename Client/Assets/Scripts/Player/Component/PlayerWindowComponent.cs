using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerWindowComponent : MonoComponent<PrefabLinkedUISystem>
{
	[SerializeField] private ThirdPersonController controller = null;

    private BattleMainWindow battleWindow = null;


	private void OnEnable()
	{
		controller.IsUIOpened += IsUIOpened;
	}

	private void OnDisable()
	{
		controller.IsUIOpened -= IsUIOpened;
	}

	private void Start()
	{
		battleWindow = System.GetMainWindow<BattleMainWindow>(); 
	}

	private bool IsUIOpened()
	{
		if(System.GetPopupStack()>0)
			return true;
			
		return false;
	}
}
