using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InGameServer
{
    internal class InGameRoomFactory
    {
        public static InGameRoom Make(int roomId, int maxRoomMemberCount)
        {
            return (InGameRoom)RoomManager.Instance.Make(roomId, maxRoomMemberCount, MakeRoom);
        }

        private static Room MakeRoom(int roomId, int maxRoomMemberCount)
        {
            return new InGameRoom(roomId, maxRoomMemberCount);
        }
    }
}
