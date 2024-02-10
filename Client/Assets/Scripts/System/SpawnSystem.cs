using Mirror;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class SpawnSystem : NetworkSystem
{
	[SerializeField] private Cinemachine.CinemachineVirtualCamera playerCamera = null;
	[SerializeField] private PlayerInputAsset inputAsset = null;

	[SerializeField] private CharacterType characterType = CharacterType.MAX;
	[SerializeField] private PlayerComponent playerPrefab = null;

	private int myPlayerId = -1;
	private List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

	/// <summary>
	/// 클라, 서버 공통 로직
	/// </summary>
	/// <param name="sceneModuleParam"></param>
	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		if (sceneModuleParam is BattleSceneModule.Param battleParam)
		{
			myPlayerId = battleParam.myPlayerId;
			playerInfoList = battleParam.playerInfoList;
        }
	}

	[Client]
    public override void OnClientConnected()
    {
        base.OnClientConnected();

		var myPlayer = FindObjectsOfType<PlayerComponent>().FirstOrDefault(p => p.playerId == myPlayerId);
		if (myPlayer == null)
			return;

        myPlayer.SetInput(inputAsset);
        myPlayer.SetCamera(playerCamera);
    }
}
