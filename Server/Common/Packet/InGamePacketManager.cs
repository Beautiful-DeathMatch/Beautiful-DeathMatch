using ServerCore;
using System;
using System.Collections.Generic;

public class InGamePacketManager : Singleton<InGamePacketManager>
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
		_makeFunc.Add((ushort)InGamePacketID.REQ_ENTER_GAME, MakePacket<REQ_ENTER_GAME>);
		_handler.Add((ushort)InGamePacketID.REQ_ENTER_GAME, PacketHandler.ON_REQ_ENTER_GAME);
		_makeFunc.Add((ushort)InGamePacketID.REQ_LEAVE_GAME, MakePacket<REQ_LEAVE_GAME>);
		_handler.Add((ushort)InGamePacketID.REQ_LEAVE_GAME, PacketHandler.ON_REQ_LEAVE_GAME);
		_makeFunc.Add((ushort)InGamePacketID.REQ_PLAYER_LIST, MakePacket<REQ_PLAYER_LIST>);
		_handler.Add((ushort)InGamePacketID.REQ_PLAYER_LIST, PacketHandler.ON_REQ_PLAYER_LIST);
		_makeFunc.Add((ushort)InGamePacketID.REQ_TRANSFORM, MakePacket<REQ_TRANSFORM>);
		_handler.Add((ushort)InGamePacketID.REQ_TRANSFORM, PacketHandler.ON_REQ_TRANSFORM);
		_makeFunc.Add((ushort)InGamePacketID.REQ_ANIMATOR, MakePacket<REQ_ANIMATOR>);
		_handler.Add((ushort)InGamePacketID.REQ_ANIMATOR, PacketHandler.ON_REQ_ANIMATOR);

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
			HandlePacket(session, packet);

			onRecvCallback?.Invoke(session, packet);					
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