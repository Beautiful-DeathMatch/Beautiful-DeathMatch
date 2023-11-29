using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
	public void OnReceive(IPacket packet);
}

public abstract class SyncComponent : MonoBehaviour, IPacketReceiver
{
	protected int playerId;

	public bool IsEqualPlayer(SyncComponent component)
	{
		if (component == null)
			return false;

		return this.playerId == component.playerId;
	}

	public virtual void Initialize(int playerId = -1)
	{
		this.playerId = playerId;
		SessionManager.Instance.RegisterPacketReceiver(this);
	}

	public virtual void Clear()
	{
		SessionManager.Instance.UnRegisterPacketReceiver(this);
	}

	public abstract void OnReceive(IPacket packet);
}