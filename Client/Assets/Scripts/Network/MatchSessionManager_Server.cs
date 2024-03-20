using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MatchSessionManager
{
	private Dictionary<NetworkConnectionToClient, Guid> playerMatches = new Dictionary<NetworkConnectionToClient, Guid>();
	private Dictionary<NetworkConnection, PlayerInfo> playerInfoDictionary = new Dictionary<NetworkConnection, PlayerInfo>();
	private Dictionary<Guid, HashSet<NetworkConnectionToClient>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnectionToClient>>();
	private List<NetworkConnectionToClient> waitingConnections = new List<NetworkConnectionToClient>();

	[ServerCallback]
	public override void OnStartServer()
	{
		InitializeData();
		NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
	}

	[ServerCallback]
	public override void OnServerReady(NetworkConnectionToClient conn)
	{
		base.OnServerReady(conn);

		waitingConnections.Add(conn);
		playerInfoDictionary.Add(conn, new PlayerInfo { playerId = conn.connectionId, selectedCharacterType = 1, ready = false });

		SendMatchList();
	}

	[ServerCallback]
	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		Guid matchId;
		if (playerMatches.TryGetValue(conn, out matchId))
		{
			playerMatches.Remove(conn);
			openMatches.Remove(matchId);

			foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
			{
				PlayerInfo _playerInfo = playerInfoDictionary[playerConn];
				_playerInfo.ready = false;
				_playerInfo.matchId = Guid.Empty;
				playerInfoDictionary[playerConn] = _playerInfo;
				playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
			}
		}

		foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> kvp in matchConnections)
			kvp.Value.Remove(conn);

		PlayerInfo playerInfo = playerInfoDictionary[conn];
		if (playerInfo.matchId != Guid.Empty)
		{
			MatchInfo matchInfo;
			if (openMatches.TryGetValue(playerInfo.matchId, out matchInfo))
			{
				matchInfo.players--;
				openMatches[playerInfo.matchId] = matchInfo;
			}

			HashSet<NetworkConnectionToClient> connections;
			if (matchConnections.TryGetValue(playerInfo.matchId, out connections))
			{
				PlayerInfo[] infos = connections.Select(playerConn => playerInfoDictionary[playerConn]).ToArray();

				foreach (NetworkConnectionToClient playerConn in matchConnections[playerInfo.matchId])
					if (playerConn != conn)
						playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
			}
		}

		SendMatchList();
	}

	[ServerCallback]
	void OnServerMatchMessage(NetworkConnectionToClient conn, ServerMatchMessage msg)
	{
		switch (msg.serverMatchOperation)
		{
			case ServerMatchOperation.None:
				{
					Debug.LogWarning("Missing ServerMatchOperation");
					break;
				}
			case ServerMatchOperation.Create:
				{
					OnServerCreateMatch(conn);
					break;
				}
			case ServerMatchOperation.Cancel:
				{
					OnServerCancelMatch(conn);
					break;
				}
			case ServerMatchOperation.Start:
				{
					OnServerMatchStart(conn, msg.matchId);
					break;
				}
			case ServerMatchOperation.Join:
				{
					OnServerJoinMatch(conn, msg.matchId);
					break;
				}
			case ServerMatchOperation.Leave:
				{
					OnServerLeaveMatch(conn, msg.matchId);
					break;
				}
			case ServerMatchOperation.Ready:
				{
					OnServerPlayerReady(conn, msg.matchId);
					break;
				}
		}
	}

	[ServerCallback]
	void OnServerMatchStart(NetworkConnectionToClient conn, Guid matchId)
	{
		// 플레이 중인 매치로 변경하기 혹은 따로 관리하기

		// 방에 참여 중인 유저에게 현재 방 정보를 노티하기
		HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
		PlayerInfo[] infos = connections.Select(playerConn => playerInfoDictionary[playerConn]).ToArray();

		foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
		{
			playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Started, myPlayerId = playerConn.connectionId,  playerInfos = infos });
		}
	}

	[ServerCallback]
	void OnServerPlayerReady(NetworkConnectionToClient conn, Guid matchId)
	{
		PlayerInfo playerInfo = playerInfoDictionary[conn];
		playerInfo.ready = !playerInfo.ready;
		playerInfoDictionary[conn] = playerInfo;

		HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
		PlayerInfo[] infos = connections.Select(playerConn => playerInfoDictionary[playerConn]).ToArray();

		foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
			playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
	}

	[ServerCallback]
	void OnServerLeaveMatch(NetworkConnectionToClient conn, Guid matchId)
	{
		MatchInfo matchInfo = openMatches[matchId];
		matchInfo.players--;

		openMatches[matchId] = matchInfo;

		PlayerInfo playerInfo = playerInfoDictionary[conn];
		playerInfo.ready = false;
		playerInfo.matchId = Guid.Empty;
		playerInfoDictionary[conn] = playerInfo;

		foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> kvp in matchConnections)
		{
			kvp.Value.Remove(conn);
		}

		HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
		PlayerInfo[] infos = connections.Select(playerConn => playerInfoDictionary[playerConn]).ToArray();

		foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
		{
			playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
		}

		SendMatchList();

		conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
	}

	[ServerCallback]
	void OnServerCreateMatch(NetworkConnectionToClient conn)
	{
		if (playerMatches.ContainsKey(conn)) return;

		Guid newMatchId = Guid.NewGuid();
		matchConnections.Add(newMatchId, new HashSet<NetworkConnectionToClient>());
		matchConnections[newMatchId].Add(conn);
		playerMatches.Add(conn, newMatchId);
		openMatches.Add(newMatchId, new MatchInfo { matchId = newMatchId, maxPlayers = 2, players = 1 });

		PlayerInfo playerInfo = playerInfoDictionary[conn];
		playerInfo.ready = false;
		playerInfo.matchId = newMatchId;
		playerInfoDictionary[conn] = playerInfo;

		PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfoDictionary[playerConn]).ToArray();

		conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, playerInfos = infos });

		SendMatchList();
	}

	[ServerCallback]
	void OnServerCancelMatch(NetworkConnectionToClient conn)
	{
		if (!playerMatches.ContainsKey(conn)) return;

		conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Cancelled });

		Guid matchId;
		if (playerMatches.TryGetValue(conn, out matchId))
		{
			playerMatches.Remove(conn);
			openMatches.Remove(matchId);

			foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
			{
				PlayerInfo playerInfo = playerInfoDictionary[playerConn];
				playerInfo.ready = false;
				playerInfo.matchId = Guid.Empty;
				playerInfoDictionary[playerConn] = playerInfo;
				playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
			}

			SendMatchList();
		}
	}

	[ServerCallback]
	void OnServerJoinMatch(NetworkConnectionToClient conn, Guid matchId)
	{
		if (!matchConnections.ContainsKey(matchId) || !openMatches.ContainsKey(matchId)) return;

		MatchInfo matchInfo = openMatches[matchId];
		matchInfo.players++;
		openMatches[matchId] = matchInfo;
		matchConnections[matchId].Add(conn);

		PlayerInfo playerInfo = playerInfoDictionary[conn];
		playerInfo.ready = false;
		playerInfo.matchId = matchId;
		playerInfoDictionary[conn] = playerInfo;

		PlayerInfo[] infos = matchConnections[matchId].Select(playerConn => playerInfoDictionary[playerConn]).ToArray();
		SendMatchList();

		conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Joined, matchId = matchId, playerInfos = infos });

		foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
		{
			playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
		}
	}

	/// <summary>
	/// Sends updated match list to all waiting connections or just one if specified
	/// </summary>
	/// <param name="conn"></param>
	[ServerCallback]
	internal void SendMatchList(NetworkConnectionToClient conn = null)
	{
		if (conn != null)
		{
			conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
		}
		else
		{
			foreach (NetworkConnectionToClient waiter in waitingConnections)
			{
				waiter.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
			}
		}
	}

}
