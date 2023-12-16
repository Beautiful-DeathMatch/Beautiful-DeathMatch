using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnSystem : SyncComponent
{
	[SerializeField] private Cinemachine.CinemachineVirtualCamera playerCamera = null;
	[SerializeField] private StarterAssetsInputs inputAsset = null;
	[SerializeField] private PlayerInput inputComponent = null;

	[SerializeField] private PlayerComponent playerPrefab = null;
	private Dictionary<int, PlayerComponent> playerDictionary = new Dictionary<int, PlayerComponent>();

	private void Awake()
	{
		playerDictionary.Clear();
		Initialize();
	}

	private void OnDestroy()
	{
		Clear();
	}

	private PlayerComponent CreateController(int playerId, bool isSelf, Vector3 initialPos)
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
		playerComponent.SetPosition(transform.position);

		return playerComponent;
	}

	public override void OnReceive(IPacket packet)
	{
		if(packet is RES_PLAYER_LIST playerListPacket)
		{
			foreach (var id in GetLeftPlayerIds(playerListPacket.players))
			{
				if (playerDictionary.TryGetValue(id, out var controller))
				{
					playerDictionary.Remove(id);
					Destroy(controller.gameObject);
				}
			}

			foreach (var player in playerListPacket.players)
			{
				if (playerDictionary.ContainsKey(player.playerId))
					continue;

				var controller = CreateController(player.playerId, player.isSelf, new Vector3(player.posX, player.posY, player.posZ));
				playerDictionary[player.playerId] = controller;
			}
		}
	}

	private IEnumerable<int> GetLeftPlayerIds(IEnumerable<RES_PLAYER_LIST.Player> players)
	{
		foreach (var playerComponent in playerDictionary.Values)
		{
			if (playerComponent == null)
				yield break;

			bool isContain = players.Any(p => p.playerId == playerComponent.playerId);
			if (isContain)
				continue;

			yield return playerComponent.playerId;
		}
	}
}
