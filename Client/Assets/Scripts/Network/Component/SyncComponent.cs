using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyncComponent : MonoBehaviour
{
	public int PlayerId { get; private set; }

	public virtual void Initialize(int playerId)
	{
		this.PlayerId = playerId;
	}

	private void OnEnable()
	{
		NetworkManager.Instance.RegisterComponent(this);
	}

	private void OnDisable()
	{
		NetworkManager.Instance.UnregisterComponent(this);
	}

	public abstract void TrySend();

	public abstract void OnReceive(IPacket packet);
}