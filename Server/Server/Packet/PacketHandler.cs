using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

public class PacketHandler
{
	public static void ON_REQ_BROADCAST_ENTER_GAME(PacketSession session, IPacket packet)
	{
		
	}

    internal static void ON_REQ_BROADCAST_LEAVE_GAME(PacketSession session, IPacket packet)
    {
		
	}

    internal static void ON_REQ_PLAYER_LIST(PacketSession session, IPacket packet)
	{
		if (session is IngameSession ingameSession)
		{
			ingameSession.TryBroadcastPlayerList();
		}
	}
	
	internal static void ON_REQ_LEAVE_GAME(PacketSession session, IPacket packet)
	{
		if (session is IngameSession ingameSession)
		{
			ingameSession.TryLeave();
		}
	}

    internal static void ON_REQ_MOVE(PacketSession session, IPacket packet)
	{
		var movePacket = packet as REQ_MOVE;

		if (session is IngameSession ingameSession)
		{
			ingameSession.TryMove(movePacket);
		}
	}

}
