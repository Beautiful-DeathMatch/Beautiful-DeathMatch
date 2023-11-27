using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace ServerCore
{
	public class SessionConnector
	{
		public bool IsConnected { get; private set; } = false;

		private IPEndPoint currentEndPoint;
		private Func<Session> _sessionFactory;

		private ProtocolType protocolType;

		private int reconnectCount = 0;
		private int maxReconnectCount = 0;

		public SessionConnector(ProtocolType protocolType, int maxReconnectCount)
		{
			this.protocolType = protocolType;
			this.maxReconnectCount = maxReconnectCount;
		}

		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
		{
            IsConnected = false;

			currentEndPoint = endPoint;

			// 휴대폰 설정
			var socket = new Socket(currentEndPoint.AddressFamily, SocketType.Stream, protocolType);
            _sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = currentEndPoint;
            args.UserToken = socket;

            RegisterConnect(args);
        }

		void RegisterConnect(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;

			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.ConnectSocket);
				session.OnConnected(args.RemoteEndPoint);

				IsConnected = true;
			}
			else
			{
				Debug.LogError($"OnConnectCompleted Fail: {args.SocketError}, 연결 재시도 횟수 : {reconnectCount++}");

				if (reconnectCount < maxReconnectCount)
				{
					Connect(currentEndPoint, _sessionFactory);
				}
				else
				{
					Debug.LogError($"OnConnectCompleted Fail: {args.SocketError}, 재시도 횟수를 모두 사용하여 연결 종료합니다.");
				}
			}
		}
	}
}
