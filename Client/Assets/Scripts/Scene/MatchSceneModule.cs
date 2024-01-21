using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSceneModule : NetworkSceneModule
{
	[SerializeField] private MatchBoardMainWindow matchBoardCanvas;

#if UNITY_EDITOR
	protected override NetworkManager CreateNetworkManager()
	{
        var networkManager = FindObjectOfType<MatchSessionManager>();
        if (networkManager == null)
        {
            var g = new GameObject("MatchSessionManager");
            g.transform.SetParent(transform);

            networkManager = g.AddComponent<MatchSessionManager>();
        }
        return networkManager;
    }

    protected override Transport CreateTransport()
    {
        var transport = FindObjectOfType<TelepathyTransport>();
        if (transport == null)
        {
            var g = new GameObject("TelepathyTransport");
            g.transform.SetParent(transform);

            transport = g.AddComponent<TelepathyTransport>();
        }

        return transport;
    }
#endif

	public override void OnEnter(SceneModuleParam param)
	{
		base.OnEnter(param);

		MatchSessionManager.Instance.SubscribeMatchInfo(matchBoardCanvas);
	}

	public override void OnExit()
	{
		base.OnExit();
		MatchSessionManager.Instance.UnSubscribeMatchInfo(matchBoardCanvas);
	}

	public override void OnStartClient()
	{
		matchBoardCanvas.OnStartClient();
	}

	public override void OnStopClient()
	{
		matchBoardCanvas.OnStopClient();
	}

	public override void OnClientConnected()
	{
		
	}

	public override void OnClientDisconnected()
	{
		matchBoardCanvas.OnClientDisconnect();
	}
}

