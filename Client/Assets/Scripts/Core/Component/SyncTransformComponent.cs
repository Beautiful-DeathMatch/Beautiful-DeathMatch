using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransformComponent : SyncComponent
{
    [SerializeField] private Transform myTransform = null;

    private Vector3 prevPos = Vector3.zero;
    private Vector3 prevRot = Vector3.zero;
    
	protected override bool IsSendCondition()
	{
		return IsEqualWithPrevFrame() == false;
	}
	
	private bool IsEqualWithPrevFrame()
	{
		return prevPos == myTransform.position && prevRot == myTransform.rotation.eulerAngles;
	}
	
	protected override void TrySend()
	{
		var myPos = myTransform.position;
		var myRot = myTransform.rotation.eulerAngles;
		
		var packet = new REQ_TRANSFORM();
		packet.posX = myPos.x;
		packet.posY = myPos.y;
		packet.posZ = myPos.z;
		packet.rotX = myRot.x;
		packet.rotY = myRot.y;
		packet.rotZ = myRot.z;

		Send(packet);
		
		prevPos = myPos;
		prevRot = myRot;
	}

	public override void OnReceive(IPacket packet)
	{
		if (packet is RES_TRANSFORM transformPacket == false)
			return;
		
		if (transformPacket.playerId != playerId)
			return;

		myTransform.position = new Vector3(transformPacket.posX, transformPacket.posY, transformPacket.posZ);
		myTransform.rotation = Quaternion.Euler(transformPacket.rotX, transformPacket.rotY, transformPacket.rotZ);
	}
}
