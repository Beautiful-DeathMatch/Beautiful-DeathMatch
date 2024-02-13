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
	[SerializeField] private BattleNetworkBlackBoard blackboard;

	private List<NetworkConnectionToClient> connectedPlayerInfos = new List<NetworkConnectionToClient>();

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
		var playerInfo = battleParam.GetMyPlayerInfo();

		var message = new PlayerReadyMessage();
		message.playerId = playerInfo.playerId;
		message.selectedCharacterType = playerInfo.selectedCharacterType;

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
		if (isReady)
			return;

		if (connectedPlayerInfos.Contains(conn))
			return;

		connectedPlayerInfos.Add(conn);

		var player = Instantiate(playerComponent);

		player.Initialize(message.playerId);
		player.SetCharacter((CharacterType)message.selectedCharacterType);

		NetworkServer.Spawn(player.gameObject, conn);

		if (connectedPlayerInfos.Count == battleParam.playerInfoList.Count)
		{
			var blackBoardObj = Instantiate(blackboard);
			blackBoardObj.Initialize(battleParam);

			NetworkServer.Spawn(blackBoardObj.gameObject);
			NetworkServer.SendToAll(new PlayerAllReadyMessage());
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		base.OnServerDisconnect(conn);

		isReady = false;

		if (connectedPlayerInfos.Contains(conn))
		{
			connectedPlayerInfos.Remove(conn);
		}
	}

	protected override void RegisterClientMessages()
	{
		base.RegisterClientMessages();

		NetworkClient.RegisterHandler<PlayerAllReadyMessage>(OnAllPlayerReadyMessage);
	}

	private void OnAllPlayerReadyMessage(PlayerAllReadyMessage message)
	{
		isReady = true;
	}
}
