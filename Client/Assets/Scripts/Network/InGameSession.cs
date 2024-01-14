using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public class InGameSession : PacketSession
{
    public InGameSession(int sessionId) : base(sessionId)
    {

    }

    public override void OnConnected(EndPoint endPoint)
    {
        UnityEngine.Debug.Log($"OnConnected : {endPoint}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
		UnityEngine.Debug.Log($"OnDisconnected : {endPoint}");
	}

    public override void OnReceivePacket(ArraySegment<byte> buffer)
    {
        InGamePacketManager.Instance.OnReceivePacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
		UnityEngine.Debug.Log($"Transferred bytes: {numOfBytes}");
    }
}