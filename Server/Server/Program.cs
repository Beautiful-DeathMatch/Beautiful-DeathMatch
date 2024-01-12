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

		private static JobTimer jobTimer = new JobTimer();
		private static int flushWaitTime = 250;

		private static void FlushMyRoom()
		{
			InGameRoomManager.Instance.Flush();

			jobTimer.Push(FlushMyRoom, flushWaitTime);
		}

		private static void Main(string[] args)
		{
			myIPEndPoint = GetMyEndPoint(myPortNumber);
			if (myIPEndPoint == null)
				return;

			mylistener.Start(myIPEndPoint, MakeSession);
			jobTimer.Start(FlushMyRoom);

			while (true)
			{
				jobTimer.Tick();
			}
		}

		private static Session MakeSession(EndPoint clientEndPoint)
		{
			return InGameSessionFactory.Make(clientEndPoint.GetHashCode(), MakeRoom);
		}

		private static InGameRoom MakeRoom(int roomId, int maxRoomMemberCount)
		{
			return InGameRoomManager.Instance.Make(roomId, maxRoomMemberCount);
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
