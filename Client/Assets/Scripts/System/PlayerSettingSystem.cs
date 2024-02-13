using Cysharp.Threading.Tasks;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSettingSystem : MonoSystem
{
	[SerializeField] private Cinemachine.CinemachineVirtualCamera playerCamera = null;
	[SerializeField] private PlayerInputAsset inputAsset = null;

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		if (param is BattleSceneModule.Param battleParam)
		{
			var myPlayer = FindObjectsOfType<PlayerComponent>().FirstOrDefault(p => p.playerId == battleParam.myPlayerId);
			if (myPlayer == null)
				return;

			myPlayer.transform.SetParent(transform, false);
			myPlayer.transform.position = transform.position;

			await base.OnPrepareEnterRoutine(param);
			await UniTask.Yield();

			myPlayer.SetAnimator();
			myPlayer.SetCamera(playerCamera);
			myPlayer.SetInput(inputAsset);
		}
	}
}
