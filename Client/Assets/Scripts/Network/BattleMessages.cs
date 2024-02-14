using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerReadyMessage : NetworkMessage
{
	public int playerId;
	public int selectedCharacterType;
}

public struct PlayerAllReadyMessage : NetworkMessage
{
	public NetworkPlayerInfo[] playerInfos;
}

[System.Serializable]
public struct NetworkPlayerInfo
{
	public int netId;
	public int playerId;
}