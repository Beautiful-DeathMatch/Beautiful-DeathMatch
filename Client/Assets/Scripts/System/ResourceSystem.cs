using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceSystem : MonoSystem
{
	public T Load<T>(string path = "") where T : Object
	{
		if (path == null || path == string.Empty)
		{
			path = Utility.GetResourcePath<T>();
		}

		return Resources.Load<T>(path);
	}

	public T Instantiate<T>(string path = "") where T : Object
	{
		var prefab = Load<T>(path);
		if (prefab == null)
			return null;

		return Instantiate(prefab);
	}

	public void Unload(Object obj)
	{
		Resources.UnloadAsset(obj);
	}
}
