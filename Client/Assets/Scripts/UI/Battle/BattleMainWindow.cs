using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class BattleMainWindow : UIMainWindow
{
    private int myPlayerId = -1;
    private PlayerComponent myPlayerComponent = null;
    
    public override void OnEnter(SceneModuleParam param)
    {
		if (param is BattleSceneModule.Param battleParam)
        {
            myPlayerId = battleParam.myPlayerId;
            myPlayerComponent = FindObjectsOfType<PlayerComponent>().FirstOrDefault(com => com.playerId == myPlayerId);
        }
	}

    private void PrintCurrentActiveText()
    {
       
    }

	private void PrintWeaponListText()
    {
        
    }

	private void PrintItemListText()
    {
        
    }

    private void PrintMissionText()
    {

    }

	private void PrintInteractionText()
    {

    }

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

		PrintCurrentActiveText();
		PrintWeaponListText();
		PrintItemListText();
		PrintMissionText();
		PrintInteractionText();
	}
}
