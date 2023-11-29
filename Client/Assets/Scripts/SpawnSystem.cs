using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnSystem : SyncComponent
{
	private Dictionary<int, PlayableController> controllerDictionary = new Dictionary<int, PlayableController>();

	private void Awake()
	{
		controllerDictionary.Clear();
		Initialize();
	}

	private void OnDestroy()
	{
		Clear();
	}

	private PlayableController CreateController(int playerId, Vector3 initialPos)
	{
		var g = GameManager.Resource.Instantiate("Character/Player", transform);
		if (g == null)
			return null;

		var controller = g.GetComponent<PlayableController>();
		if (controller == null)
			return null;

		controller.Initialize(playerId);
		controller.Move(initialPos);

		return controller;
	}

	public override void OnReceive(IPacket packet)
	{
		if (packet is RES_BROADCAST_ENTER_GAME enterPacket)
		{
			if (controllerDictionary.ContainsKey(enterPacket.playerId))
				return;

			var controller = CreateController(enterPacket.playerId, new Vector3(enterPacket.posX, enterPacket.posY, enterPacket.posZ));
			if (controller == null)
				return;

			controllerDictionary[enterPacket.playerId] = controller;
		}
		else if(packet is RES_BROADCAST_LEAVE_GAME leavePacket)
		{
			if(controllerDictionary.TryGetValue(leavePacket.playerId, out var controller))
			{
				controllerDictionary.Remove(leavePacket.playerId);
				Destroy(controller);
			}
		}
		else if(packet is RES_PLAYER_LIST playerListPacket)
		{
			foreach (var player in playerListPacket.players)
			{
				if (controllerDictionary.ContainsKey(player.playerId))
					continue;

				var controller = CreateController(player.playerId, new Vector3(player.posX, player.posY, player.posZ));
				controllerDictionary[player.playerId] = controller;
			}
		}
	}
}
