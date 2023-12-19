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
			ingameSession.OnRequestPlayerList();
		}
	}
	
	internal static void ON_REQ_LEAVE_GAME(PacketSession session, IPacket packet)
	{
		if (session is IngameSession inGameSession)
		{
			inGameSession.OnRequestLeaveGame();
		}
	}

    internal static void ON_REQ_TRANSFORM(PacketSession session, IPacket packet)
	{
		var movePacket = packet as REQ_TRANSFORM;

		if (session is IngameSession inGameSession)
		{
			inGameSession.OnRequestTransform(movePacket);
		}
	}

	internal static void ON_REQ_ANIMATOR(PacketSession session, IPacket packet)
	{
		var movePacket = packet as REQ_ANIMATOR;

		if (session is IngameSession inGameSession)
		{
			inGameSession.OnRequestAnimator(movePacket);
		}
	}

}
