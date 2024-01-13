using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomServer
{
    internal class OutGameRoom : Room
    {
        internal OutGameRoom(int roomId, int maxSessionCount) : base(roomId, maxSessionCount)
        {
        }
    }
}
