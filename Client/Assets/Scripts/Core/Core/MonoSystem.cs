using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSystem : MonoSubSystem
{
	[SerializeField] private bool dontDestroySystemOnLoad = false;

	private void Awake()
	{
		OnAwake();

		if (dontDestroySystemOnLoad)
		{
			DontDestroyOnLoad(this);
		}
    }

	private void Start()
	{
		OnStart();
    }

	private void Update()
	{
		OnUpdate();
	}

	private void LateUpdate()
	{
		OnLateUpdate();
	}

	private void FixedUpdate()
	{
		OnFixedUpdate();
	}

	private void OnApplicationQuit()
	{
		OnDispose();
	}

    private void OnDestroy()
    {
        OnDispose();
    }

    protected override bool OnDispose()
    {
		if (base.OnDispose() == false)
			return false;

		return true;
	}
}
