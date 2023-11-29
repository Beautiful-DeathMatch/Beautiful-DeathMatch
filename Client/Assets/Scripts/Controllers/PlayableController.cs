using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayableController : SyncComponent
{
	private IEnumerable<SyncComponent> childSyncComponents = null;

	public override void Initialize(int playerId)
	{
		base.Initialize(playerId);

		childSyncComponents = GetComponentsInChildren<SyncComponent>().Where(c => c != this);
		foreach (var child in childSyncComponents)
		{
			child.Initialize(playerId);
		}
	}

	public override void Clear()
	{
		base.Clear();

		foreach (var child in childSyncComponents)
		{
			child.Clear();
		}
	}

	public void Move(Vector3 targetPos)
    {
        transform.position = targetPos;
    }

	public override void OnReceive(IPacket packet)
	{
		
	}
}
