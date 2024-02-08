using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttribute : ResourceAttribute
{
    public EntityAttribute(string path) : base("Prefabs/Entity/" + path, ResourceType.Prefab)
    {

    }
}

public class MapAttribute : ResourceAttribute
{
	public MapAttribute(string path) : base("Prefabs/Map/" + path, ResourceType.Prefab)
	{

	}
}

public class PopupAttribute : UIAttribute
{
	public PopupAttribute(string path) : base("Popup/" + path)
	{

	}
}

public class UIAttribute : ResourceAttribute
{
	public UIAttribute(string path) : base("Prefabs/UI/" + path, ResourceType.Prefab)
	{

	}
}

public class AIAttribute : ResourceAttribute
{
    public AIAttribute(string path) : base("Prefabs/AI/" + path, ResourceType.Prefab)
    {
    }
}

public class ResourceAttribute : Attribute
{
	public readonly string resourcePath = string.Empty;
	public readonly ResourceType resourceType = ResourceType.UnityAsset;

	public ResourceAttribute(string path, ResourceType type)
	{
		this.resourcePath = path;
		this.resourceType = type;
	}
}

public enum ResourceType
{
	UnityAsset = 0,
	Prefab = 1,
}

public static class Utility
{
	public static bool TryParse<TEnum>(string enumValue, out TEnum result) where TEnum : struct, Enum
	{
		if (Enum.TryParse(enumValue, out result))
		{
			return true;
		}

		return false;
	}

	public static IEnumerable<Type> GetSubClassTypes<T>()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach(var type in assembly.GetTypes())
			{
				if (type.IsSubclassOf(typeof(T)))
				{
					yield return type;
				}
			}
		}
	}

	public static string GetResourcePath<T>() where T : UnityEngine.Object
	{
		Type type = typeof(T);

		Attribute[] attributes = Attribute.GetCustomAttributes(type);

		foreach (Attribute attr in attributes)
		{
			ResourceAttribute resourceAttr = attr as ResourceAttribute;

			if (resourceAttr != null)
			{
				return resourceAttr.resourcePath;
			}
		}

		return string.Empty;
	}

	public static string GetResourcePath(Type type)
	{
		Attribute[] attributes = Attribute.GetCustomAttributes(type);

		foreach (Attribute attr in attributes)
		{
			ResourceAttribute resourceAttr = attr as ResourceAttribute;

			if (resourceAttr != null)
			{
				return resourceAttr.resourcePath;
			}
		}

		return string.Empty;
	}

	public static ResourceType GetResourceType<T>() where T : UnityEngine.Object
	{
		Type type = typeof(T);

		Attribute[] attributes = Attribute.GetCustomAttributes(type);

		foreach (Attribute attr in attributes)
		{
			ResourceAttribute resourceAttr = attr as ResourceAttribute;

			if (resourceAttr != null)
			{
				return resourceAttr.resourceType;
			}
		}

		return ResourceType.UnityAsset;
	}

	public static ResourceType GetResourceType(Type type)
	{
		Attribute[] attributes = Attribute.GetCustomAttributes(type);

		foreach (Attribute attr in attributes)
		{
			ResourceAttribute resourceAttr = attr as ResourceAttribute;

			if (resourceAttr != null)
			{
				return resourceAttr.resourceType;
			}
		}

		return ResourceType.UnityAsset;
	}

}
