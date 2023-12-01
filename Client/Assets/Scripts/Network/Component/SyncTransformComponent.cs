using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformComponent : SyncComponent
{
    [SerializeField] private Transform myTransform = null;

	private void Update()
	{
		// TrySend();
	}

	public void TrySend()
	{
		var myPos = myTransform.position;

		var movePacket = new REQ_MOVE();
		movePacket.posX = myPos.x;
		movePacket.posY = myPos.y;
		movePacket.posZ = myPos.z;

		SessionManager.Instance.Send(movePacket);
	}

	public override void OnReceive(IPacket packet)
	{
		if (packet is RES_MOVE movePacket == false)
			return;

		if (movePacket == null)
			return;

		if (movePacket.playerId != playerId)
			return;

		// myTransform.position = new Vector3(movePacket.posX, movePacket.posY, movePacket.posZ);
	}
}
