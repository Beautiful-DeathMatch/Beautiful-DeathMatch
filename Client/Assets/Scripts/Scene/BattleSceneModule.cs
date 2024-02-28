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

		public int GetMyIndex()
		{
			for (int i = 0; i < playerInfoList.Count; i++)
			{
				if (myPlayerId == playerInfoList[i].playerId)
					return i;
			}

			return -1;
		}

		public PlayerInfo GetMyPlayerInfo()
		{
			return GetPlayerInfo(myPlayerId);
		}

		public PlayerInfo GetPlayerInfo(int playerId)
		{
			return playerInfoList.FirstOrDefault(p => p.playerId == playerId);
		}
	}

	private BattleNetworkBlackBoard networkBlackBoard;

	[SerializeField] private PlayerSettingSystem playerSettingSystem = null;
	[SerializeField] private ItemSystem itemSystem = null;
	[SerializeField] private StatusSystem statusSystem = null;
	[SerializeField] private MissionSystem missionSystem = null;
    [SerializeField] private PrefabLinkedUISystem uiSystem = null;
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
		if (networkBlackBoard == null)
			return;

		networkBlackBoard.OnStartSync();

		if (itemSystem)
		{
			itemSystem.OnStartSync(networkBlackBoard);
		}
		if (statusSystem)
		{
			statusSystem.OnStartSync(networkBlackBoard);
		}
		if (missionSystem)
		{
			missionSystem.OnStartSync(networkBlackBoard);
		}

		base.OnEnter(param);

		if (playerSettingSystem)
		{
			playerSettingSystem.OnEnter(param);
		}
		if (itemSystem)
		{
			itemSystem.OnEnter(param);
		}
		if (statusSystem)
		{
			statusSystem.OnEnter(param);
		}
		if(missionSystem)
		{
			missionSystem.OnEnter(param);
		}
		if (uiSystem)
		{
			uiSystem.OnEnter(param);
		}
	}

	public override void OnExit()
	{
		base.OnExit();

		if (networkBlackBoard != null)
			networkBlackBoard.OnStopClient();

		if (itemSystem != null)
		{
			itemSystem.OnStopSync();
		}
		if(statusSystem != null)
		{
			statusSystem.OnStopSync();
		}
		if (missionSystem != null)
		{
			missionSystem.OnStopSync();
		}

		// 로직 추가
	}

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await base.OnPrepareEnterRoutine(param); // 네트워크 연결 시도

		if (param is BattleSceneModule.Param battleParam)
		{
			while (currentSession.IsReady == false)
			{
				await UniTask.Yield();
			}

			while (networkBlackBoard == null)
			{
				networkBlackBoard = FindObjectOfType<BattleNetworkBlackBoard>();
				await UniTask.Yield();
			}

			if (playerSettingSystem)
			{
				await playerSettingSystem.OnPrepareEnterRoutine(param);
			}
		}
	}

	public override UniTask OnPrepareExitRoutine()
	{
		// 로직 추가
		return base.OnPrepareExitRoutine();
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		base.OnUpdate(deltaFrameCount, deltaTime);

		if (uiSystem)
		{
			uiSystem.OnUpdate(deltaFrameCount, deltaTime);
		}

		// 로직 추가
	}
}
