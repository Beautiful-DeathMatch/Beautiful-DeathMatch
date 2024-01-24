using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class BattleMainWindow : UIMainWindow
{
    [SerializeField] private ItemSystem itemSystem;
    [SerializeField] private MissionSystem missionSystem;
    [SerializeField] private StatusSystem statusSystem;

    private int myPlayerId = -1;
    
    public override void OnEnter(SceneModuleParam param)
    {
		if (param is BattleSceneModule.Param battleParam)
        {
            myPlayerId = battleParam.myPlayerId;
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
