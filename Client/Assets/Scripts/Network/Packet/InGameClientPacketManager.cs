using ServerCore;
using System;
using System.Collections.Generic;

public partial class InGamePacketManager : Singleton<InGamePacketManager>
{
	protected override void OnAwakeInstance()
    {
        base.OnAwakeInstance();
        RegisterHandler();
    }

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void RegisterHandler()
	{
		_makeFunc.Add((ushort)InGamePacketID.RES_CONNECTED, MakePacket<RES_CONNECTED>);
		_handler.Add((ushort)InGamePacketID.RES_CONNECTED, PacketHandler.ON_RES_CONNECTED);
		_makeFunc.Add((ushort)InGamePacketID.RES_BROADCAST_ENTER_GAME, MakePacket<RES_BROADCAST_ENTER_GAME>);
		_handler.Add((ushort)InGamePacketID.RES_BROADCAST_ENTER_GAME, PacketHandler.ON_RES_BROADCAST_ENTER_GAME);
		_makeFunc.Add((ushort)InGamePacketID.RES_BROADCAST_LEAVE_GAME, MakePacket<RES_BROADCAST_LEAVE_GAME>);
		_handler.Add((ushort)InGamePacketID.RES_BROADCAST_LEAVE_GAME, PacketHandler.ON_RES_BROADCAST_LEAVE_GAME);
		_makeFunc.Add((ushort)InGamePacketID.RES_PLAYER_LIST, MakePacket<RES_PLAYER_LIST>);
		_handler.Add((ushort)InGamePacketID.RES_PLAYER_LIST, PacketHandler.ON_RES_PLAYER_LIST);
		_makeFunc.Add((ushort)InGamePacketID.RES_TRANSFORM, MakePacket<RES_TRANSFORM>);
		_handler.Add((ushort)InGamePacketID.RES_TRANSFORM, PacketHandler.ON_RES_TRANSFORM);
		_makeFunc.Add((ushort)InGamePacketID.RES_ANIMATOR, MakePacket<RES_ANIMATOR>);
		_handler.Add((ushort)InGamePacketID.RES_ANIMATOR, PacketHandler.ON_RES_ANIMATOR);

	}

	public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onReceiveCallback = null)
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

			onReceiveCallback?.Invoke(session, packet);					
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
	}
}