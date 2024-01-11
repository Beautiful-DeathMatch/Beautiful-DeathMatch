using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
	public void OnReceive(IPacket packet);
}

public abstract class SyncComponent : MonoComponent<SyncSystem>, IPacketReceiver
{
	public int playerId { get; private set; }

	private SyncSystem syncSystem = null;

	public virtual void Initialize(int playerId = -1)
	{
		this.playerId = playerId;

		syncSystem = FindSystem();
		if (syncSystem == null)
			return;

		syncSystem.Register(this);
	}

	public virtual void Clear()
	{
		this.playerId = 0;

		if (syncSystem != null)
		{
			syncSystem.UnRegister(this);
		}
	}

	private void Update()
	{
		if (IsSendCondition())
		{
			TrySend();
		}
	}

	protected virtual bool IsSendCondition()
	{
		return true;
	}

	protected virtual void TrySend()
	{

	}

	protected void Send(IPacket packet)
	{
		syncSystem.Send(packet);
	}

	public abstract void OnReceive(IPacket packet);
}