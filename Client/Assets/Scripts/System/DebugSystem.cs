using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem; // For Debug

public class DebugSystem : MonoSystem   
{

    [SerializeField] PlayerInputAsset starterAssetsInputs;

    public override void OnEnter(SceneModuleParam sceneModuleParam)
    {
        base.OnEnter(sceneModuleParam);

        if (sceneModuleParam is BattleSceneModule.Param param)
        {
           
        }
    }

	private void OnGUI()
	{
		if (GUI.Button(new Rect(300, 0, 300, 150), "디버그 모드 시작"))
		{
			starterAssetsInputs.cursorLocked = true;
			starterAssetsInputs.cursorInputForLook = true;
		}
	}
}
