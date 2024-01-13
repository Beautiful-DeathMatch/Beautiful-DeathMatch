using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ServerCore
{
    internal interface IProgram
    {
        public Session MakeSession(EndPoint clientEndPoint);
        public Room MakeRoom(int roomId, int maxRoomMemberCount);
    }
}
