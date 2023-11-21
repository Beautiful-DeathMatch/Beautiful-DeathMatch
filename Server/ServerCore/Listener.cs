using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	public class Listener
	{
		private Socket listenSocket;

		private Func<Session> _sessionFactory;
		private Func<Room> _roomFactory;

		private int acceptCount = 0;
		private int backLogCount = 0;

		public Listener(int listenCount, int backLogCount)
		{
			this.acceptCount = listenCount;
			this.backLogCount = backLogCount;
		}

		public void Start(IPEndPoint endPoint, Func<Session> sessionFactory, Func<Room> roomFactory)
		{
			listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			_sessionFactory = sessionFactory;
			_roomFactory = roomFactory;

			listenSocket.Bind(endPoint);
			listenSocket.Listen(backLogCount);

			TryAcceptAsyncAll(acceptCount);
			Console.WriteLine($"{endPoint}에서 {acceptCount} 개의 소켓 리스닝 시작...");
		}

		private void TryAcceptAsyncAll(int acceptCount)
		{
			for (int i = 0; i < acceptCount; i++)
			{
				var args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

				TryAcceptAsync(args);
			}
		}

		private void TryAcceptAsync(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			bool pending = listenSocket.AcceptAsync(args);
			if (pending == false)
			{
				OnAcceptCompleted(null, args);
			}	
		}

		private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint, _roomFactory.Invoke());
			}
			else
			{
				Console.WriteLine(args.SocketError.ToString());
			}
				
			TryAcceptAsync(args);
		}
	}
}
