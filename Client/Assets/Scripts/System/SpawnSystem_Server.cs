using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SpawnSystem
{
    /// <summary>
    /// 서버용 딕셔너리
    /// </summary>
    private Dictionary<int, PlayerComponent> playerDictionary = new Dictionary<int, PlayerComponent>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        playerDictionary.Clear();
        foreach (var playerInfo in playerInfoList)
        {
            var player = CreatePlayer(playerInfo.playerId, (CharacterType)playerInfo.selectedCharacterType, transform.position);
            if (player == null)
                return;

            NetworkServer.Spawn(player.gameObject);
            playerDictionary.Add(playerInfo.playerId, player);
        }
    }

    [Server]
    private PlayerComponent CreatePlayer(int playerId, CharacterType characterType, Vector3 initialPos)
    {
        var playerComponent = Instantiate(playerPrefab, transform);
        if (playerComponent == null)
            return null;

        playerComponent.SetPlayerId(playerId);
        playerComponent.SetCharacter(characterType);
        playerComponent.SetPosition(initialPos);

        return playerComponent;
    }
}
