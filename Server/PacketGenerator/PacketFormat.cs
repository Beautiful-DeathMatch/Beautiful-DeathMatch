using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
	class PacketFormat
	{
		// {0} 패킷 핸들러 등록
		// {1} 패킷 매니저 종류
		public static string ManagerScriptFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;

public partial class {1}PacketManager : Singleton<{1}PacketManager>
{{
	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

	public event Action<PacketSession, IPacket> _eventHandler = null;

	protected override void OnAwakeInstance()
    {{
        base.OnAwakeInstance();
        RegisterHandler();
    }}

	public void RegisterHandler()
	{{
{0}
	}}

	public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer)
	{{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{{
			IPacket packet = func.Invoke(session, buffer);
			HandlePacket(session, packet);
		}}
	}}

	private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}}

	public void HandlePacket(PacketSession session, IPacket packet)
	{{
		if (_handler.TryGetValue(packet.Protocol, out var action))
		{{
			action.Invoke(session, packet);
		}}

		_eventHandler?.Invoke(session, packet);
	}}
}}";

		// {0} 패킷 이름
		public static string HandlerFormat =
@"		_makeFunc.Add((ushort){0}PacketID.{1}, MakePacket<{1}>);
		_handler.Add((ushort){0}PacketID.{1}, ON_{1});";

		// {0} 패킷 이름/번호 목록
		// {1} 패킷 목록
		// {2} 패킷 종류
		public static string ScriptFormat =
@"using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum {2}PacketID
{{
	{0}
}}

{1}
";

		// {0} 패킷 이름
		// {1} 패킷 번호
		public static string packetEnumFormat =
@"{0} = {1},";


		// {0} 패킷 이름
		// {1} 멤버 변수들
		// {2} 멤버 변수 Read
		// {3} 멤버 변수 Write
		public static string PacketProtocolFormat =
@"
public class {0} : IPacket
{{
	{1}

	public ushort Protocol {{ get {{ return (ushort){4}PacketID.{0}; }} }}

	public void Read(ArraySegment<byte> segment)
	{{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		{2}
	}}

	public ArraySegment<byte> Write()
	{{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort){4}PacketID.{0}), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		{3}

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}}
}}
";
		// {0} 변수 형식
		// {1} 변수 이름
		public static string MemberFormat =
@"public {0} {1};";

		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		// {2} 멤버 변수들
		// {3} 멤버 변수 Read
		// {4} 멤버 변수 Write
		public static string FieldFormat =
@"public class {0}
{{
	{2}

	public void Read(ArraySegment<byte> segment, ref ushort count)
	{{
		{3}
	}}

	public bool Write(ArraySegment<byte> segment, ref ushort count)
	{{
		bool success = true;
		{4}
		return success;
	}}	
}}
public List<{0}> {1}s = new List<{0}>();";

		// {0} 변수 이름
		// {1} To~ 변수 형식
		// {2} 변수 형식
		public static string ReadFunctionFormat =
@"this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";

		// {0} 변수 이름
		// {1} 변수 형식
		public static string ReadByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

		// {0} 변수 이름
		public static string ReadStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";

		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		public static string ReadListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
	{0} {1} = new {0}();
	{1}.Read(segment, ref count);
	{1}s.Add({1});
}}";

		// {0} 변수 이름
		// {1} 변수 형식
		public static string WriteFunctionFormat =
@"Array.Copy(BitConverter.GetBytes(this.{0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});";

		// {0} 변수 이름
		// {1} 변수 형식
		public static string WriteByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

		// {0} 변수 이름
		public static string WriteStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
count += {0}Len;";

		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		public static string WriteListFormat =
@"Array.Copy(BitConverter.GetBytes((ushort)this.{1}s.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
foreach ({0} {1} in this.{1}s)
	{1}.Write(segment, ref count);";

	}
}
