using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject g) where T : Component
	{
		T component = g.GetComponent<T>();

		if (component == null)
		{
			component = g.AddComponent<T>();
		}

		return component;
	}
}
