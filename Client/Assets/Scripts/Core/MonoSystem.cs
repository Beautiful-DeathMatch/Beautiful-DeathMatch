using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public static class AssetLoadHelper
{
	private const string SystemAssetPathFormat = "Assets/Prefabs/Resources/System/{0}Asset.asset";

	public static string GetSystemAssetPath<T>() where T : MonoSystem
	{
		return GetSystemAssetPath(typeof(T));
	}

	public static string GetSystemAssetPath(Type systemType)
	{
		return string.Format(SystemAssetPathFormat, systemType);
	}
	public static TSystem GetSystemAsset<TSystem>() where TSystem : MonoSystem
	{
		return AssetDatabase.LoadAssetAtPath<TSystem>(GetSystemAssetPath<TSystem>());
	}
}
#endif

public static class SystemHelper
{
	public static void SetChildObject(this MonoSystem system, MonoBehaviour childObj)
	{
		SceneModuleSystemManager.Instance.SetSystemChild(system, childObj);
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
