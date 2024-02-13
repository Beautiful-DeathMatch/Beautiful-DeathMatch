using Cysharp.Threading.Tasks;
using Mirror;
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

	private PlayerComponent myPlayerComponent;

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		if (param is BattleSceneModule.Param battleParam)
		{
			while (myPlayerComponent == null)
			{
				myPlayerComponent = GetMyPlayerComponent();
				await UniTask.Yield();
			}

			myPlayerComponent.Initialize(battleParam.myPlayerId);
			myPlayerComponent.SetCharacter((CharacterType)battleParam.GetMyPlayerInfo().selectedCharacterType);

			await UniTask.Yield();
			myPlayerComponent.transform.SetParent(transform, false);
			myPlayerComponent.transform.position = transform.position;

			await base.OnPrepareEnterRoutine(param);
			await UniTask.Yield();

			myPlayerComponent.SetAnimator();
			myPlayerComponent.SetCamera(playerCamera);
			myPlayerComponent.SetInput(inputAsset);
		}
	}

	private PlayerComponent GetMyPlayerComponent()
	{
		foreach(var player in FindObjectsOfType<PlayerComponent>())
		{
			var identity = player.GetComponent<NetworkIdentity>();
			if (identity == null)
				continue;

			if (identity.isOwned == false)
				continue;

			return player;
		}

		return null;
	}

	public override void OnExit()
	{
		base.OnExit();

		if (myPlayerComponent != null)
		{
			myPlayerComponent.UnsetAnimator();
		}
	}
}
