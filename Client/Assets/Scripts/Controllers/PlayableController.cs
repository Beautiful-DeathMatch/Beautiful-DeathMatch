using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableController : SyncComponent
{
    public void Move(Vector3 targetPos)
    {
        transform.position = targetPos;
    }

	public override void OnReceive(IPacket packet)
	{
		
	}

	public override void TrySend()
	{
		
	}
}
