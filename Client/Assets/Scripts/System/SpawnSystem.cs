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

	[SerializeField] private CharacterType characterType = CharacterType.MAX;
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

	public void TryConnect()
	{
		SessionManager.Instance.TryConnect((bResult) =>
		{
			if (bResult)
			{
				var enterPacket = new REQ_ENTER_GAME();
				enterPacket.characterType = (int)characterType;
				SessionManager.Instance.Send(enterPacket);
			}
		});
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

	public override void OnReceive(IPacket packet)
	{
		if (packet is RES_BROADCAST_ENTER_GAME enterPacket ||
		    packet is RES_BROADCAST_LEAVE_GAME leavePacket)
		{
			REQ_PLAYER_LIST req = new REQ_PLAYER_LIST();
			SessionManager.Instance.Send(req);
		}
		else if(packet is RES_PLAYER_LIST playerListPacket)
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

				var controller = CreatePlayer(player.playerId, player.isSelf, (CharacterType)player.characterType, transform.position);
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
