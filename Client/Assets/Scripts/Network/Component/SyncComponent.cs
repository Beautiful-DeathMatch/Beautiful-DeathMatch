using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncComponent<TPacket> : MonoBehaviour 
\	where TPacket : IPacket
{
	protected int playerId = 0;

	public void Initialize(int playerId)
	{
		this.playerId = playerId;
	}

	public virtual void Send()
	{

	}

	public virtual void OnReceive(TPacket packet) 
	{ 
	
	}
}
