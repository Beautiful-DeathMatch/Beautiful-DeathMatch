using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 300, 100), "네트워크 연결 및 캐릭터 스폰"))
		{
			TryConnect();
		}

		characterType = (CharacterType)GUI.Toolbar(new Rect(350, 30, 300, 30), (int)characterType, new string[(int)CharacterType.MAX]
		{
			CharacterType.CH_03.ToString(),
			CharacterType.CH_29.ToString(),
			CharacterType.CH_46.ToString()
		});
	}

	public override void Clear()
	{
		base.Clear();

		SessionManager.Instance.Disconnect();
	}

	public void TryConnect()
	{
		SessionManager.Instance.TryConnect();
	}

	private void Update()
	{
		SessionManager.Instance.OnUpdateInstance();
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
		if(packet is RES_CONNECTED connectedPacket)
		{
			var enterPacket = new REQ_ENTER_GAME();
			enterPacket.characterType = (int)characterType;
			SessionManager.Instance.Send(enterPacket);
		}
		else if (packet is RES_BROADCAST_ENTER_GAME enterPacket ||
		    packet is RES_BROADCAST_LEAVE_GAME leavePacket)
		{
			REQ_PLAYER_LIST req = new REQ_PLAYER_LIST();
			SessionManager.Instance.Send(req);
		}
		else if(packet is RES_PLAYER_LIST playerListPacket)
		{
			var leftPlayerIds = GetLeftPlayerIds(playerListPacket.players).ToList();
			
			foreach (var id in leftPlayerIds)
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
