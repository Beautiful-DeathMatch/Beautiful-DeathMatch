using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class SessionSystem : MonoSystem
{
	private PacketSession packetSession = null;
	private SessionConnector connector = null;

	private List<IPacketReceiver> packetReceivers = new();
	private Queue<IPacketReceiver> pendingPacketReceiverQueue = new();

	[SerializeField] private SessionType sessionType;
	[SerializeField] private ProtocolType protocolType = ProtocolType.Tcp;

	[SerializeField] private int reconnectCount = 5;

	[Header("네트워크 상태에 따라 체크되는 변수들")]
	[SerializeField] private bool isConnected = false;

    protected override void OnAwake()
    {
        base.OnAwake();

        connector = new SessionConnector(ProtocolType.Tcp, reconnectCount);
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();

		isConnected = connector?.IsConnected ?? false;
    }

    public void Send(IPacket packet)
	{
		if (packetSession == null)
		{
			Debug.LogError($"{packet.GetType()} : 세션이 활성화되지 않은 상태에서 패킷 전송을 시도합니다.");
			return;
		}

		packetSession.Send(packet.Write());
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

	public bool TryConnect()
	{
		var endPoint = GetMyEndPoint(GetPortNumber(sessionType));
		if (endPoint == null)
			return false;

		if (connector == null)
			return false;

		if (connector.IsConnected)
			return false;

		connector.Connect(endPoint, MakeSession);
		return true;
	}

	public void Disconnect()
	{
		connector.Disconnect();
	}

	protected override void OnUpdate()
	{
		if (IsConnectedSession() == false)
			return;

		FlushSession();
	}

	private void FlushSession()
	{
		while (pendingPacketReceiverQueue.TryDequeue(out var pendingReceiver))
		{
			if (packetReceivers.Contains(pendingReceiver))
				continue;

			packetReceivers.Add(pendingReceiver);
		}

		foreach (IPacket packet in PacketSessionHandler.Flush())
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
			var session = SessionFactory.Instance.Make(sessionType);
			if (session is PacketSession pSession)
			{
				packetSession = pSession;
				return packetSession;
			}
		}

		return packetSession;
	}

	private IPEndPoint GetMyEndPoint(int portNumber)
	{
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddress = ipHost.AddressList.FirstOrDefault();

		return new IPEndPoint(ipAddress, portNumber);
	}

	private int GetPortNumber(SessionType type)
	{
		switch(type)
		{
			case SessionType.Login:
				return 7776;

			case SessionType.InGame:
				return 7777;

			case SessionType.Room:
				return 7778;
		}

		return 7777;
	}
}
