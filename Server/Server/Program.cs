using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    internal class Program
	{
		private static IPEndPoint myIPEndPoint = null;
		private static int myPortNumber = 7777;

		private static Listener mylistener = new Listener(ProtocolType.Tcp, 10, 100);
		private static List<Room> myRooms = new List<Room>();

		private static JobTimer jobTimer = new JobTimer();
		private static int flushWaitTime = 250;

		private static void FlushMyRoom()
		{
			foreach(var room in myRooms)
			{
				room.Flush();
			}

			jobTimer.Push(FlushMyRoom, flushWaitTime);
		}

		private static void Main(string[] args)
		{
			myIPEndPoint = GetMyEndPoint(myPortNumber);
			if (myIPEndPoint == null)
				return;

			mylistener.Start(myIPEndPoint, MakeSession, MakeRoom);
			jobTimer.Start(FlushMyRoom);

			while (true)
			{
				jobTimer.Tick();
			}
		}

		private static Session MakeSession()
		{
			return SessionFactory.Instance.Make(SessionType.InGame);
		}

		private static Room MakeRoom()
		{
			foreach(var room in myRooms)
			{
				if (room.IsFull == false)
				{
					return room;
				}
			}

			var newRoom = RoomFactory.Instance.Make(RoomType.InGame);
			myRooms.Add(newRoom);

			return newRoom;
		}

		private static IPEndPoint GetMyEndPoint(int portNumber)
		{
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHost.AddressList.FirstOrDefault();

			return new IPEndPoint(ipAddress, portNumber);
		}
	}
}
