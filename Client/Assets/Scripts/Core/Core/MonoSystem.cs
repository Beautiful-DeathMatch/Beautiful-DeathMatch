using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSystem : MonoSubSystem
{
	private void Awake()
	{
		OnAwake();
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
}
