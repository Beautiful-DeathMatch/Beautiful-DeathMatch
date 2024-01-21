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
	private BattleSceneModule.Param battleParam = null;

	private ISessionSubscriber subscriber = null;

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
		
	}

	public override void OnStopClient()
	{
		
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();
	}

	public override void OnClientDisconnect()
	{
		base.OnClientDisconnect();
	}
}
