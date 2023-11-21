using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace Server
{
    public class ClientSession : PacketSession
	{
		private InGameRoom sessionRoom;

		public ClientSession(int sessionId) : base(sessionId)
		{

		}

		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }

		public bool TryLeave()
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Leave(this);
			return true;
		}

		public bool TryMove(C_Move movePacket)
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Move(this, movePacket);
			return true;
		}

		public override void OnConnected(EndPoint endPoint, Room room)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			sessionRoom = room as InGameRoom;
			sessionRoom?.Enter(this);
		}

		public override void OnReceivePacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionFactory.Instance.Remove(this);

			sessionRoom?.Leave(this);
			sessionRoom = null;

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			
		}
	}
}
