using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
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
	public T Load<T>() where T : Object
	{
		return Resources.Load<T>("");
	}

	public ResourceRequest LoadAsync<T>(string path) where T : Object
	{
		return Resources.LoadAsync<T>(path);
	}

	public T Instantiate<T>() where T : Object
	{
		var prefab = Load<T>();
		return MonoBehaviour.Instantiate<T>(prefab);
	}

	public void Unload(Object obj)
	{
		Resources.UnloadAsset(obj);
	}

}
