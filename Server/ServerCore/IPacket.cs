using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public interface IPacket
    {
        ushort Protocol { get; }
        void Read(ArraySegment<byte> segment);
        ArraySegment<byte> Write();
    }
}
