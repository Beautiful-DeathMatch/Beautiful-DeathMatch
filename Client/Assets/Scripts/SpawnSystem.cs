using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnSystem : SyncComponent
{
	private Dictionary<int, PlayableController> controllerDictionary = new Dictionary<int, PlayableController>();

	public override void OnReceive(IPacket packet)
	{
		if (packet is RES_BROADCAST_ENTER_GAME enterPacket)
		{
			var g = GameManager.Resource.Instantiate("Character/Player", transform);
			if (g == null)
				return;

			var controller = g.GetComponent<PlayableController>();
			if (controller == null)
				return;

			controller.Initialize(enterPacket.playerId);
			controller.Move(new Vector3(enterPacket.posX, enterPacket.posY, enterPacket.posZ));

			controllerDictionary[enterPacket.playerId] = controller;
		}
		else if(packet is RES_BROADCAST_LEAVE_GAME leavePacket)
		{
			if(controllerDictionary.ContainsKey(leavePacket.playerId))
			{
				Destroy(controllerDictionary[leavePacket.playerId]);
				controllerDictionary.Remove(leavePacket.playerId);
			}
		}
	}

	public override void TrySend()
	{
		
	}
}
