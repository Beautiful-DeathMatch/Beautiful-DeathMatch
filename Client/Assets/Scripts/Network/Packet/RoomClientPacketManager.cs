using ServerCore;
using System;
using System.Collections.Generic;

public partial class RoomPacketManager : Singleton<RoomPacketManager>
{
	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

	public event Action<PacketSession, IPacket> _eventHandler = null;

	protected override void OnAwakeInstance()
    {
        base.OnAwakeInstance();
        RegisterHandler();
    }

	public void RegisterHandler()
	{
		_makeFunc.Add((ushort)RoomPacketID.RES_CREATE_ROOM, MakePacket<RES_CREATE_ROOM>);
		_handler.Add((ushort)RoomPacketID.RES_CREATE_ROOM, ON_RES_CREATE_ROOM);

	}

	public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{
			IPacket packet = func.Invoke(session, buffer);
			HandlePacket(session, packet);
		}
	}

	private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}

	public void HandlePacket(PacketSession session, IPacket packet)
	{
		if (_handler.TryGetValue(packet.Protocol, out var action))
		{
			action.Invoke(session, packet);
		}

		_eventHandler?.Invoke(session, packet);
	}
}