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
    public class IngameSession : PacketSession
	{
		private InGameRoom sessionRoom;

		public IngameSession(int sessionId) : base(sessionId)
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
			sessionRoom.BroadcastPlayerList(this);

			return true;
		}

		public bool TryMove(REQ_MOVE movePacket)
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Move(this, movePacket);
			return true;
		}

		public bool TryBroadcastPlayerList()
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.BroadcastPlayerList(this);
			return true;
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");
		}

        public override void OnConnectedRoom(Room room)
        {
			sessionRoom = room as InGameRoom;
            sessionRoom?.Enter(this);
			sessionRoom?.BroadcastPlayerList(this);
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
