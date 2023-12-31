﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public class IngameSession : PacketSession
{
    public IngameSession(int sessionId) : base(sessionId)
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

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) =>
        {
            SessionManager.Instance.Receive(p);
        });
    }

    public override void OnSend(int numOfBytes)
    {
		UnityEngine.Debug.Log($"Transferred bytes: {numOfBytes}");
    }
}