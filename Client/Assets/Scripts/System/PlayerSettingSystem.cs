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

	[SerializeField] private CharacterType characterType = CharacterType.MAX;
	[SerializeField] private PlayerComponent playerPrefab = null;

	private Dictionary<int, PlayerComponent> playerDictionary = new Dictionary<int, PlayerComponent>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		
	}

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await base.OnPrepareEnterRoutine(param);

		playerDictionary.Clear();

		// 스폰된 플레이어의 id로 맞추어 세팅해줄 수 있는 방법을 찾자
		if (param is BattleSceneModule.Param battleParam)
		{
			var spawnedPlayerComponents = FindObjectsOfType<PlayerComponent>();
			if (spawnedPlayerComponents == null)
				return;

			if (spawnedPlayerComponents.Length != battleParam.playerInfoList.Count)
				return;

			for (int i = 0; i < battleParam.playerInfoList.Count; i++)
			{
				var playerComponent = spawnedPlayerComponents[i];
				var playerInfo = battleParam.playerInfoList[i];

				playerComponent.Initialize(playerInfo.playerId);
				playerComponent.SetCharacter(characterType);

				playerComponent.transform.SetParent(transform, false);
				playerComponent.transform.position = transform.position;

				await UniTask.Yield();

				if (playerInfo.playerId == battleParam.myPlayerId)
				{
					playerComponent.SetInput(inputAsset);
					playerComponent.SetCamera(playerCamera);
				}

				playerDictionary.Add(playerInfo.playerId, playerComponent);
			}
		}
	}
}
