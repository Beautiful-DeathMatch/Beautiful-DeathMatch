using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using System.Net.Sockets;

public static class PacketSessionHandler
{
	private static PacketQueue packetQueue = new PacketQueue();

	public static void Push(IPacket packet)
	{
		packetQueue.Push(packet);
	}

	public static IEnumerable<IPacket> Flush()
	{
		return packetQueue.PopAll();
	}
}
