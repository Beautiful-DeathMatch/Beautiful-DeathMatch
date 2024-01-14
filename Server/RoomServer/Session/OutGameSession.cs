using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace RoomServer
{
    internal class OutGameSession : PacketSession
	{
		private OutGameRoom sessionRoom;
		private Func<int, int, Room> roomFactory;

        internal OutGameSession(int sessionId, Func<int, int, Room> roomFactory) : base(sessionId)
		{
			this.roomFactory = roomFactory;
		}

        public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");
		}

        public override void OnReceivePacket(ArraySegment<byte> buffer)
		{
			RoomPacketManager.Instance.OnReceivePacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.UnRegisterSession(this);

			sessionRoom?.Leave(this);
			sessionRoom = null;

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			
		}
	}
}
