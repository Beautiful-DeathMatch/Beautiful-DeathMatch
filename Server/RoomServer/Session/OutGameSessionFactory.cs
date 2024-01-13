using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace RoomServer
{
	public class OutGameSessionFactory
	{
		public static Session Make(int clientHashCode, Func<int, int, OutGameRoom> roomFactory)
		{
			var session = new OutGameSession(clientHashCode, roomFactory);
			if (session == null)
				return null;

			SessionManager.Instance.RegisterSession(session);
			return session;
		}
	}
}
