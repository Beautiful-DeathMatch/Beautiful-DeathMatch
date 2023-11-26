﻿using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
	private PacketQueue packetQueue = new PacketQueue();
    private PacketSession packetSession = null;

    private SessionConnector connector = new SessionConnector();

    public void Send(IPacket packet)
	{
        packetSession.Send(packet.Write());
	}

	public void Receive(IPacket packet)
	{
		packetQueue.Push(packet);
	}

    public bool TryConnect()
    {
		var endPoint = GetMyEndPoint(7777);
        if (endPoint == null)
            return false;

		connector.Connect(endPoint, MakeSession);
        return true;
	}

    public override void OnUpdateInstance()
    {
        if (connector == null)
            return;

        if (connector.IsConnected == false)
            return;

		foreach (IPacket packet in packetQueue.PopAll())
		{
            PacketManager.Instance.HandlePacket(packetSession, packet);
        }
    }

    private PacketSession MakeSession()
    {
        if (packetSession == null)
        {
			var session = SessionFactory.Instance.Make(SessionType.InGame);
			if (session is PacketSession pSession)
			{
				packetSession = pSession;
				return packetSession;
			}
		}
		
        return packetSession;
    }

    private static IPEndPoint GetMyEndPoint(int portNumber)
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddress = ipHost.AddressList.FirstOrDefault();

        return new IPEndPoint(ipAddress, portNumber);
    }
}
