using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager : Singleton<PacketManager>
{
	protected override void OnAwakeInstance()
    {
        base.OnAwakeInstance();
        Register();
    }

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.RES_BROADCAST_ENTER_GAME, MakePacket<RES_BROADCAST_ENTER_GAME>);
		_handler.Add((ushort)PacketID.RES_BROADCAST_ENTER_GAME, PacketHandler.ON_RES_BROADCAST_ENTER_GAME);
		_makeFunc.Add((ushort)PacketID.RES_BROADCAST_LEAVE_GAME, MakePacket<RES_BROADCAST_LEAVE_GAME>);
		_handler.Add((ushort)PacketID.RES_BROADCAST_LEAVE_GAME, PacketHandler.ON_RES_BROADCAST_LEAVE_GAME);
		_makeFunc.Add((ushort)PacketID.RES_PLAYER_LIST, MakePacket<RES_PLAYER_LIST>);
		_handler.Add((ushort)PacketID.RES_PLAYER_LIST, PacketHandler.ON_RES_PLAYER_LIST);
		_makeFunc.Add((ushort)PacketID.RES_MOVE, MakePacket<RES_MOVE>);
		_handler.Add((ushort)PacketID.RES_MOVE, PacketHandler.ON_RES_MOVE);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
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
			if (onRecvCallback != null)
				onRecvCallback.Invoke(session, packet);
			else
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
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(packet.Protocol, out action))
			action.Invoke(session, packet);
	}
}