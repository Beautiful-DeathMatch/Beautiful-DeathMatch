using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemHelper
{
	public static void SetChildObject(this MonoSystem system, MonoBehaviour childObj)
	{
		SceneModuleSystemManager.Instance.SetSystemChild(system, childObj);
	}
}

public interface IMonoSystem
{
	public void OnEnter(SceneModuleParam sceneModuleParam);

	public void OnExit();

	public void OnUpdate(int deltaFrameCount, float deltaTime);

	public void OnClientConnected();
	public void OnClientDisconnected();
}

public abstract class NetworkSystem : NetworkBehaviour, IMonoSystem
{
	protected SceneModuleParam sceneModuleParam = null;

    protected override void OnValidate()
    {
        gameObject.GetOrAddComponent<NetworkIdentity>();
        base.OnValidate();
    }

	public virtual void OnClientConnected()
	{

	}

	public virtual void OnClientDisconnected()
	{

	}


    public virtual void OnEnter(SceneModuleParam sceneModuleParam)
    {
		this.sceneModuleParam = sceneModuleParam;	
    }

    public virtual void OnExit()
    {

    }

    public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
    {

    }
}

public abstract class MonoSystem : MonoBehaviour, IMonoSystem
{
	private void Reset()
	{
		OnReset();
	}

	protected virtual void OnReset()
	{

	}

    public virtual void OnClientConnected()
    {

    }

    public virtual void OnClientDisconnected()
    {

    }

    public virtual void OnEnter(SceneModuleParam sceneModuleParam)
	{

	}

	public virtual void OnExit()
	{

	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}
