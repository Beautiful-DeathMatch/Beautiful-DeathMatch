using Cysharp.Threading.Tasks;
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

public abstract class NetworkSystem : MonoSystem
{
	protected BattleNetworkBlackBoard blackBoard;

	public void OnStartSync(BattleNetworkBlackBoard blackBoard)
	{
		this.blackBoard = blackBoard;
	}

	public void OnStopSync()
	{
		this.blackBoard = null;
	}
}

public abstract class MonoSystem : MonoBehaviour
{
	private void Reset()
	{
		OnReset();
	}

	protected virtual void OnReset()
	{

	}

	public async virtual UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await UniTask.Yield();
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
