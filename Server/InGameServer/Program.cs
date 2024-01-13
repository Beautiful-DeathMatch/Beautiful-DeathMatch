using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace InGameServer
{
    internal class Program
	{
        private static Executer executer = new Executer(MakeSession, 7777);

		private static Session MakeSession(EndPoint clientEndPoint)
		{
			return InGameSessionFactory.Make(clientEndPoint.GetHashCode(), MakeRoom);
		}

		private static InGameRoom MakeRoom(int roomId, int maxRoomMemberCount)
		{
			return InGameRoomFactory.Make(roomId, maxRoomMemberCount);
		}

        private static void Main(string[] args)
        {
            executer.Execute();
        }
    }
}
