using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainWindow : UIMainWindow
{
    [SerializeField] private ItemSystem itemSystem;
    [SerializeField] private MissionSystem missionSystem;
    [SerializeField] private StatusSystem statusSystem;

    [SerializeField] private Image[] itemImages = null; 

    private int myPlayerId = -1;
    
    public override void OnEnter(SceneModuleParam param)
    {
		if (param is BattleSceneModule.Param battleParam)
        {
            myPlayerId = battleParam.myPlayerId;
        }
	}

    public void PrintPlayerItemSlot()
    {
        var playerItemSprites = itemSystem.GetPlayerItemSprites(myPlayerId).ToArray();

        if (itemImages.Length != playerItemSprites.Length)
        {
            Debug.LogError("실제 아이템 슬롯과 UI에 표기되는 슬롯의 수가 다릅니다.");
            return;
        }

        for (int i = 0; i < playerItemSprites.Length; i++)
        {
            if(playerItemSprites[i] == null)
            {
                itemImages[i].sprite = null;
                itemImages[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                itemImages[i].sprite = playerItemSprites[i];
                itemImages[i].color = new Color(0, 0, 0, 1);
            }
        }
    }

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

        PrintPlayerItemSlot();
    }
}
