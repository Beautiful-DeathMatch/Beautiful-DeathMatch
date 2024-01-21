using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using kcp2k;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public abstract class NetworkSceneModule : SceneModule, ISessionSubscriber
{
	protected ISessionComponent currentSession;
    protected Transport currentTransport;

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        ResetNetworkConfig();
    }

    private void ResetNetworkConfig()
    {
        var networkManager = CreateNetworkManager();
        var transport = CreateTransport();

        DefineConfig(networkManager, transport);

        EditorSceneManager.SaveScene(gameObject.scene);
    }

    protected virtual void DefineConfig(NetworkManager networkConfig, Transport transportConfig)
    {
        networkConfig.transport = transportConfig;
        networkConfig.dontDestroyOnLoad = false;
        networkConfig.runInBackground = true;
    }

    protected abstract NetworkManager CreateNetworkManager();
    protected abstract Transport CreateTransport();
#endif

    public override void OnEnter(SceneModuleParam param)
    {
        base.OnEnter(param);

        currentTransport = FindObjectOfType<Transport>();
        currentSession = FindObjectOfType<NetworkManager>() as ISessionComponent;

        try
        {
            currentTransport.Initialize();
            currentSession.Initialize();
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}에 네트워크 매니저 혹은 트랜스포트가 존재하지 않습니다.");
            return;
        }

        currentSession.Connect(param, this);
        
    }

	public override void OnExit()
    {
        base.OnExit();

        try
        {
            currentSession.Disconnect();
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}의 네트워크 세션이 이미 종료되었습니다.");
            return;
        }

    }

	public virtual void OnStartClient()
	{
		
	}

	public virtual void OnStopClient()
	{
		
	}

	public virtual void OnClientConnected()
	{
		
	}

	public virtual void OnClientDisconnected()
	{
		
	}
}
