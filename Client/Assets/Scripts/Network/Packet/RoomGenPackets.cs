using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum RoomPacketID
{
	REQ_CREATE_ROOM = 1,
	
}


public class REQ_CREATE_ROOM : IPacket
{
	public int roomId;
	public string roodName;
	public int roomMaxMemberCount;

	public ushort Protocol { get { return (ushort)RoomPacketID.REQ_CREATE_ROOM; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort roodNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.roodName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, roodNameLen);
		count += roodNameLen;
		this.roomMaxMemberCount = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)RoomPacketID.REQ_CREATE_ROOM), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort roodNameLen = (ushort)Encoding.Unicode.GetBytes(this.roodName, 0, this.roodName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(roodNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += roodNameLen;
		Array.Copy(BitConverter.GetBytes(this.roomMaxMemberCount), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

