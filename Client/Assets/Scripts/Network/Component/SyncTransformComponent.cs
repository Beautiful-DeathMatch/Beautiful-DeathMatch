using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformComponent : SyncComponent<RES_MOVE>
{
    [SerializeField] private Transform myTransform = null;

	private void Update()
	{
		Send();
	}

	public override void Send()
	{
		var myPos = myTransform.position;

		var movePacket = new REQ_MOVE();
		movePacket.posX = myPos.x;
		movePacket.posY = myPos.y;
		movePacket.posZ = myPos.z;

		NetworkManager.Instance.Send(movePacket);
	}

	public override void OnReceive(RES_MOVE movePacket)
	{
		if (movePacket == null)
			return;

		if (movePacket.playerId != playerId)
			return;

		myTransform.position = new Vector3(movePacket.posX, movePacket.posY, movePacket.posZ);
	}
}
