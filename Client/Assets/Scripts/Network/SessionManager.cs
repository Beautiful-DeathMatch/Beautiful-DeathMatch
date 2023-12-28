using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using System.Net.Sockets;

public class SessionManager : Singleton<SessionManager>
{
	private PacketQueue packetQueue = new PacketQueue();
    private PacketSession packetSession = null;

    private SessionConnector connector = new SessionConnector(ProtocolType.Tcp, 5);

    private List<IPacketReceiver> packetReceivers = new();
    private Queue<IPacketReceiver> pendingPacketReceiverQueue = new();

    public void Send(IPacket packet)
	{
        packetSession?.Send(packet.Write());
	}

	public void Receive(IPacket packet)
	{
		packetQueue.Push(packet);
	}

    public void RegisterPacketReceiver(IPacketReceiver receiver)
    {
		if (receiver == null)
			return;

		pendingPacketReceiverQueue.Enqueue(receiver);
	}

    public void UnRegisterPacketReceiver(IPacketReceiver receiver)
    {
		if (receiver == null)
			return;

		if (packetReceivers.Contains(receiver) == false)
			return;

        packetReceivers.Remove(receiver);
	}

    public bool TryConnect(Action<bool> onConnected = null)
    {
		var endPoint = GetMyEndPoint(7777);
        if (endPoint == null)
            return false;

		if (connector.IsConnected)
			return false;

		connector.Connect(endPoint, MakeSession, onConnected);
        return true;
	}

	public void Disconnect()
	{
		connector.Disconnect();
	}

    public override void OnUpdateInstance()
    {
        if (IsConnectedSession() == false)
            return;

		FlushSession();
	}

    private void FlushSession()
    {
		while(pendingPacketReceiverQueue.TryDequeue(out var pendingReceiver))
		{
			if (packetReceivers.Contains(pendingReceiver))
				continue;

			packetReceivers.Add(pendingReceiver);
		}

		foreach (IPacket packet in packetQueue.PopAll())
		{
			Debug.Log(packet.GetType().ToString());

			foreach (var receiver in packetReceivers)
			{
				receiver.OnReceive(packet);
			}
		}
	}

    private bool IsConnectedSession()
    {
		if (connector == null)
			return false;

		if (connector.IsConnected == false)
			return false;

        return true;
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
