using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncSystem : MonoSystem, IPacketReceiver
{
	[SerializeField] private SessionSystem sessionSystem = null;

	private List<SyncComponent> syncComponents = new List<SyncComponent>();

	public void Register(SyncComponent component)
	{
		if (syncComponents.Contains(component))
			return;

		syncComponents.Add(component);
	}

	public void UnRegister(SyncComponent component)
	{
		if (syncComponents.Contains(component) == false)
			return;

		syncComponents.Remove(component);
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		sessionSystem.RegisterPacketReceiver(this);
	}

	protected override void OnDispose()
	{
		base.OnDispose();

		sessionSystem.UnRegisterPacketReceiver(this);
	}

	public void Send(IPacket packet)
	{
		sessionSystem.Send(packet);
	}

	public void OnReceive(IPacket packet)
	{
		foreach (var component in syncComponents)
		{
			component.OnReceive(packet);
		}
	}
}
