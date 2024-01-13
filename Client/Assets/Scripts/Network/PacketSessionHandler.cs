using System.Collections.Generic;

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
