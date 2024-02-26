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
	private PlayerComponent[] playerComponents = null;

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		if (param is BattleSceneModule.Param battleParam)
		{
			playerComponents = FindObjectsOfType<PlayerComponent>();
			
			while (playerComponents.Length < battleParam.playerInfoList.Count)
			{
				await UniTask.Yield();
				playerComponents = FindObjectsOfType<PlayerComponent>();
			}

			foreach (var networkPlayerInfo in BattleSessionManager.Instance.NetworkPlayerInfos)
			{
				var player = GetPlayerComponent(networkPlayerInfo.netId);
				if (player == null)
					continue;

				player.Initialize(networkPlayerInfo.playerId);
				player.SetCharacter((CharacterType)battleParam.GetPlayerInfo(networkPlayerInfo.playerId).selectedCharacterType);

				player.transform.SetParent(transform, false);
				player.transform.position = transform.position;
			}

			await base.OnPrepareEnterRoutine(param);

			while (myPlayerComponent == null)
			{
				myPlayerComponent = GetMyPlayerComponent();
				await UniTask.Yield();
			}

			myPlayerComponent.SetAnimator();
			myPlayerComponent.SetCamera(playerCamera);
			myPlayerComponent.SetInput(inputAsset);
		}
	}

	private PlayerComponent GetPlayerComponent(int netId)
	{
		return playerComponents.FirstOrDefault(p => p.netId == netId);
	}

	private PlayerComponent GetMyPlayerComponent()
	{
		foreach(var player in playerComponents)
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
}
