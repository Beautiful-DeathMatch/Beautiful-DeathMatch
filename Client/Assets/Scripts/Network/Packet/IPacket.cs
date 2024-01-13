using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}
