using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSubSystem : MonoBehaviour
{
    private bool isDisposed = false;

    public virtual bool IsValid()
	{
		return isDisposed == false;
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

	protected virtual bool OnDispose()
	{
		return isDisposed == false;
    }
}
