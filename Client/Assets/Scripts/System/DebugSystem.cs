using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class DebugSystem : MonoSystem   
{
	[SerializeField] SceneType loadSceneType = SceneType.Battle;
    [SerializeField] PlayerInputAsset starterAssetsInputs;
	[SerializeField] CharacterType characterType;

	private void OnGUI()
	{
        if(GUI.Button(new Rect(250, 0, 300, 150), "배틀 모듈 진입 치트"))
        {
			var infos = new PlayerInfo[1];
			infos[0] = new PlayerInfo();
			infos[0].playerId = 0;
			infos[0].selectedCharacterType = (int)characterType;

			var dummyParam = new BattleSceneModule.Param(true, 0, infos);
			SceneModuleSystemManager.Instance.TryEnterSceneModule(loadSceneType, dummyParam);
		}
		else if (GUI.Button(new Rect(600, 0, 300, 150), "캐릭터 커서 제거 치트"))
		{
			//starterAssetsInputs.cursorLocked = true;
			//starterAssetsInputs.cursorInputForLook = true;
			starterAssetsInputs.ChangeCursorStateAndLook(true);
		}
	}
}
