using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 매치에서 받아온 정보를 통해 서버와 연결
/// 2. 모두 서버와 연결되었다면 노티
/// </summary>

public partial class BattleSessionManager : NetworkManager<BattleSessionManager>, ISessionComponent
{
	[SerializeField] private PlayerComponent playerComponent;

	private Dictionary<int, NetworkConnectionToClient> connectedPlayerInfos = new Dictionary<int, NetworkConnectionToClient>();

	private BattleSceneModule.Param battleParam = null;
	private ISessionSubscriber subscriber = null;

	public bool IsReady => isReady;

	private bool isReady = false;

	public void Connect(SceneModuleParam param, ISessionSubscriber subscriber)
	{
		this.subscriber = subscriber;

		if (param is BattleSceneModule.Param battleParam)
		{
			this.battleParam = battleParam;

			if (battleParam.isOwner)
			{
				StartHost();
			}
			else
			{
				StartClient();
			}
		}
	}

	public void Disconnect()
	{
		if (battleParam.isOwner)
		{
			StopHost();
		}
		else
		{
			StopClient();
		}
	}

	public override void OnStartClient() 
	{
		var message = new PlayerReadyMessage();
		message.playerId = battleParam.myPlayerId;

		NetworkClient.Send(message);

		subscriber?.OnStartClient();
	}

	public override void OnStopClient()
	{
		subscriber?.OnStopClient();
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();

		subscriber?.OnClientConnected();
	}

	public override void OnClientDisconnect()
	{
		base.OnClientDisconnect();

		subscriber?.OnClientDisconnected();	
	}

	protected override void RegisterServerMessages()
	{
		base.RegisterServerMessages();

		NetworkServer.RegisterHandler<PlayerReadyMessage>(OnPlayerReadyMessage);
	}

	private void OnPlayerReadyMessage(NetworkConnectionToClient conn, PlayerReadyMessage message)
	{
		connectedPlayerInfos[message.playerId] = conn;

		var player = Instantiate(playerComponent);

		player.Initialize(message.playerId);
		player.SetCharacter((CharacterType)message.selectedCharacterType);

		NetworkServer.Spawn(player.gameObject, conn);

		isReady = connectedPlayerInfos.Count == battleParam.playerInfoList.Count;
	}

	protected override void RegisterClientMessages()
	{
		base.RegisterClientMessages();
	}
}
