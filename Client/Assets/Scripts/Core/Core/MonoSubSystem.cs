using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSubSystem : MonoBehaviour
{
	public virtual bool IsValid()
	{
		return true;
	}

	protected virtual void OnAwake()
	{

	}

	protected virtual void OnStart()
	{

	}

	protected virtual void OnUpdate()
	{

	}

	protected virtual void OnLateUpdate()
	{

	}

	protected virtual void OnFixedUpdate()
	{

	}

	protected virtual void OnDispose()
	{

	}
}
