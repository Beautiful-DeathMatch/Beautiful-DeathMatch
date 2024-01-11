using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
	public void OnReceive(IPacket packet);
}

public abstract class SyncComponent : MonoBehaviour, IPacketReceiver
{
	public int playerId { get; private set; }

	public virtual void Initialize(int playerId = -1)
	{
		this.playerId = playerId;
		SessionManager.Instance.RegisterPacketReceiver(this);
	}

	public virtual void Clear()
	{
		this.playerId = 0;
		SessionManager.Instance.UnRegisterPacketReceiver(this);
	}

	private void Start()
	{
		if (IsSendCondition())
		{
			TrySend();
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
	
	public abstract void OnReceive(IPacket packet);
}