using Cysharp.Threading.Tasks;
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
	[SerializeField] private ItemSystem itemSystem = null;
	// 시스템 추가

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

#endif

	public override void OnEnter(SceneModuleParam param)
	{
		base.OnEnter(param);

		spawnSystem.OnEnter(param);
		itemSystem.OnEnter(param);
	}

	public override void OnExit()
	{
		base.OnExit();

		// 로직 추가
	}

	public override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		// 로직 추가
		return base.OnPrepareEnterRoutine(param);
	}

	public override UniTask OnPrepareExitRoutine()
	{
		// 로직 추가
		return base.OnPrepareExitRoutine();
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		base.OnUpdate(deltaFrameCount, deltaTime);

		// 로직 추가
	}

}
