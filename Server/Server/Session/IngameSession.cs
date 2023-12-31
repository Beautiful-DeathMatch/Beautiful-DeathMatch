﻿using System;
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

		public bool OnRequestEnterGame(REQ_ENTER_GAME enterGame)
		{
			if (sessionRoom == null)
				return false;

			if (enterGame == null)
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
		}

        public override void OnConnectedRoom(Room room)
        {
			sessionRoom = room as InGameRoom;
			if (sessionRoom == null)
			{
				Console.WriteLine($"OnConnectedRoom Failed");
				return;
			}

			sessionRoom.Ready(this);
			Console.WriteLine($"OnConnectedRoom : {room.roomId}");
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
