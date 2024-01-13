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

	public virtual void Initialize(int playerId = -1)
	{
		this.playerId = playerId;

		if (System == null)
			return;

        System.Register(this);
	}

	public virtual void Clear()
	{
		this.playerId = 0;

		if (System != null)
		{
            System.UnRegister(this);
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
		if (System)
		{
            System.Send(packet);
        }
    }

	public abstract void OnReceive(IPacket packet);
}