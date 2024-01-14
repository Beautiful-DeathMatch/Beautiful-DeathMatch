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
	// 세션 타입 대응 필요
	private PacketSession packetSession = null;

	private SessionConnector connector = null;
	private PacketQueue packetQueue = new PacketQueue();

	private List<IPacketReceiver> packetReceivers = new();
	private Queue<IPacketReceiver> pendingPacketReceiverQueue = new();


	[SerializeField] private SessionType sessionType;
	
	// 이 타입 시리얼라이즈 잘 안됨 왜 이러지
	[SerializeField] private ProtocolType protocolType = ProtocolType.Tcp;

	[SerializeField] private int reconnectCount = 5;

	[Header("네트워크 상태에 따라 체크되는 변수들")]
	[SerializeField] private bool isConnected = false;

    protected override void OnAwake()
    {
        base.OnAwake();

        connector = new SessionConnector(ProtocolType.Tcp, reconnectCount);
		InGamePacketManager.Instance._eventHandler += OnReceivePacket;
	}

	protected override bool OnDispose()
	{
		if (base.OnDispose() == false)
			return false;

		InGamePacketManager.Instance._eventHandler -= OnReceivePacket;
		return true;
	}

	protected override void OnLateUpdate()
    {
        base.OnLateUpdate();

		isConnected = connector?.IsConnected ?? false;
    }

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (IsConnectedSession() == false)
			return;

		FlushSession();
	}

	private bool IsConnectedSession()
	{
		if (connector == null)
			return false;

		if (connector.IsConnected == false)
			return false;

		return true;
	}

	private void FlushSession()
	{
		while (pendingPacketReceiverQueue.TryDequeue(out var pendingReceiver))
		{
			if (packetReceivers.Contains(pendingReceiver))
				continue;

			packetReceivers.Add(pendingReceiver);
		}

		foreach (var packet in packetQueue.PopAll())
		{
			foreach (var receiver in packetReceivers)
			{
				receiver.OnReceive(packet);
			}
		}
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

	private void OnReceivePacket(PacketSession session, IPacket packet)
	{
		packetQueue.Push(packet);
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
