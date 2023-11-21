using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

public class PacketHandler
{
	public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
	{
		if(session is ClientSession clientSession)
		{
			clientSession.TryLeave();
		}
	}

	public static void C_MoveHandler(PacketSession session, IPacket packet)
	{
		C_Move movePacket = packet as C_Move;

		if (session is ClientSession clientSession)
		{
			clientSession.TryMove(movePacket);
		}
	}
}
