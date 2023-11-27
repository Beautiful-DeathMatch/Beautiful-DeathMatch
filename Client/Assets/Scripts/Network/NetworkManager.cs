using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using System.Net.Sockets;

public class NetworkManager : Singleton<NetworkManager>
{
	private PacketQueue packetQueue = new PacketQueue();
    private PacketSession packetSession = null;

    private SessionConnector connector = new SessionConnector(ProtocolType.Tcp, 5);

    private List<int> playerIdList = new List<int>();
    private Dictionary<int, SyncComponent> syncComponentDictionary = new Dictionary<int, SyncComponent>();

    public void Send(IPacket packet)
	{
        packetSession?.Send(packet.Write());
	}

	public void Receive(IPacket packet)
	{
		packetQueue.Push(packet);
	}

    public void RegisterComponent(SyncComponent syncComponent)
	{
		if (syncComponent == null)
			return;

		syncComponentDictionary[syncComponent.PlayerId] = syncComponent;
	}

    public void UnregisterComponent(SyncComponent syncComponent)
    {
        if (syncComponent == null)
            return;

        if (syncComponentDictionary.ContainsKey(syncComponent.PlayerId))
        {
            syncComponentDictionary.Remove(syncComponent.PlayerId);
        }
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
            Debug.Log(packet.GetType().ToString());

            PacketManager.Instance.HandlePacket(packetSession, packet);

            foreach(var syncComponent in syncComponentDictionary.Values)
            {
                syncComponent.OnReceive(packet);
			}
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
