using System.Net.Sockets;
using System.Net;
using ServerCore;

namespace RoomServer
{
    internal class Program
    {
        private static Executer executer = new Executer(MakeSession, 7778);

        private static Session MakeSession(EndPoint clientEndPoint)
        {
            return OutGameSessionFactory.Make(clientEndPoint.GetHashCode(), MakeRoom);
        }

        private static OutGameRoom MakeRoom(int roomId, int maxRoomMemberCount)
        {
            return OutGameRoomFactory.Make(roomId, maxRoomMemberCount);
        }

        private static void Main(string[] args)
        {
            executer.Execute();
        }

    }
}