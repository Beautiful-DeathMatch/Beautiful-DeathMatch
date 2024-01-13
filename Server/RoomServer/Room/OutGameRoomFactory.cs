using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomServer
{
    internal class OutGameRoomFactory
    {
        public static OutGameRoom Make(int roomId, int maxRoomMemberCount)
        {
            return (OutGameRoom)RoomManager.Instance.Make(roomId, maxRoomMemberCount, MakeRoom);
        }

        private static OutGameRoom MakeRoom(int roomId, int maxRoomMemberCount)
        {
            return new OutGameRoom(roomId, maxRoomMemberCount);
        }
    }
}
