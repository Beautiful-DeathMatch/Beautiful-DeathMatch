using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSceneModule : NetworkSceneModule
{
	public class Param : SceneModuleParam
	{
		public readonly bool isOwner = false;
		public readonly int myPlayerId = -1;
		public readonly List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

		public Param(bool isOwner, int myPlayerId, PlayerInfo[] playerInfos)
		{
			this.isOwner = isOwner;
			this.myPlayerId = myPlayerId;
			this.playerInfoList = playerInfos.ToList();
		}
	}

	[SerializeField] private SpawnSystem spawnSystem = null;

#if UNITY_EDITOR
	protected override NetworkManager CreateNetworkManager()
	{
		var networkManager = FindObjectOfType<BattleSessionManager>();
		if (networkManager == null)
		{
			var g = new GameObject("BattleSessionManager");
			g.transform.SetParent(transform);

			networkManager = g.AddComponent<BattleSessionManager>();
		}
		return networkManager;
	}

	protected override Transport CreateTransport()
	{
		var transport = FindObjectOfType<KcpTransport>();
		if (transport == null)
		{
			var g = new GameObject("KcpTransport");
			g.transform.SetParent(transform);

			transport = g.AddComponent<KcpTransport>();
		}

		return transport;
	}

	protected override void OnGUI()
	{
		base.OnGUI();

		var infos = new PlayerInfo[1];
		infos[0] = new PlayerInfo();
		infos[0].playerId = 0;

		if (GUI.Button(new Rect(0, 0, 300, 200), "캐릭터 스폰"))
		{
			OnEnter(new Param(true, 0, infos));
		}
	}
#endif

	public override void OnEnter(SceneModuleParam param)
	{
		base.OnEnter(param);

		spawnSystem.OnEnter(param);
	}

}
