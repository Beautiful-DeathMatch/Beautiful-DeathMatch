using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class DebugSystem : MonoSystem   
{
    [SerializeField] PlayerInputAsset starterAssetsInputs;

	private void OnGUI()
	{
        if(GUI.Button(new Rect(250, 0, 300, 150), "배틀 모듈 진입 치트"))
        {
			var infos = new PlayerInfo[1];
			infos[0] = new PlayerInfo();
			infos[0].playerId = 0;

			var dummyParam = new BattleSceneModule.Param(true, 0, infos);
			SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Battle, dummyParam);
		}
		else if (GUI.Button(new Rect(600, 0, 300, 150), "캐릭터 커서 제거 치트"))
		{
			//starterAssetsInputs.cursorLocked = true;
			//starterAssetsInputs.cursorInputForLook = true;
			starterAssetsInputs.ChangeCursorStateAndLook(true);
		}
	}
}
