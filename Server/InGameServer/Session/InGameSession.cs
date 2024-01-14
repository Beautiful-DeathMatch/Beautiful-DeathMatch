using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace InGameServer
{
    internal class InGameSession : PacketSession
	{
		private InGameRoom sessionRoom;
		private Func<int, int, InGameRoom> roomFactory;

        internal InGameSession(int sessionId, Func<int, int, InGameRoom> roomFactory) : base(sessionId)
		{
			this.roomFactory = roomFactory;
		}

		public bool OnRequestEnterGame(REQ_ENTER_GAME enterGame)
		{
			if (enterGame == null)
				return false;

			sessionRoom = roomFactory?.Invoke(enterGame.roomId, enterGame.roomMemberCount);
			if (sessionRoom == null)
				return false;

			sessionRoom.Enter(this, enterGame);
			return true;
		}

		public bool OnRequestLeaveGame()
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Leave(this);
			return true;
		}

		public bool OnRequestTransform(REQ_TRANSFORM movePacket)
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Send(this, movePacket);
			return true;
		}

		public bool OnRequestAnimator(REQ_ANIMATOR animatorPacket)
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.Send(this, animatorPacket);
			return true;
		}

		public bool OnRequestPlayerList()
		{
			if (sessionRoom == null)
				return false;

			sessionRoom.ResponsePlayerList(this);
			return true;
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");
			Send(new RES_CONNECTED().Write());
		}

        public override void OnReceivePacket(ArraySegment<byte> buffer)
		{
			InGamePacketManager.Instance.OnReceivePacket(this, buffer);
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
