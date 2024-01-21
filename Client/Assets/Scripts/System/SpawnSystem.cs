using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnSystem : MonoSystem
{
	[SerializeField] private Cinemachine.CinemachineVirtualCamera playerCamera = null;
	[SerializeField] private StarterAssetsInputs inputAsset = null;
	[SerializeField] private PlayerInput inputComponent = null;

	[SerializeField] private CharacterType characterType = CharacterType.MAX;
	[SerializeField] private PlayerComponent playerPrefab = null;

	private Dictionary<int, PlayerComponent> playerDictionary = new Dictionary<int, PlayerComponent>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		playerDictionary.Clear();

		if (sceneModuleParam is BattleSceneModule.Param battleParam)
		{
			foreach (var playerInfo in battleParam.playerInfoList)
			{
				var player = CreatePlayer(playerInfo.playerId, playerInfo.playerId == battleParam.myPlayerId, characterType, transform.position);
				if (player == null)
					continue;

				playerDictionary.Add(playerInfo.playerId, player);
			}
		}
	}

	private PlayerComponent CreatePlayer(int playerId, bool isSelf, CharacterType characterType, Vector3 initialPos)
	{
		var playerComponent = Instantiate<PlayerComponent>(playerPrefab, transform);
		if (playerComponent == null)
			return null;

		if (isSelf)
		{
			playerComponent.SetInput(inputComponent, inputAsset);
			playerComponent.SetCamera(playerCamera);
		}

		playerComponent.Initialize(playerId);

		playerComponent.SetCharacter(characterType);
		playerComponent.SetPosition(initialPos);

		return playerComponent;
	}
}
