using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace InGameServer
{

	public class InGameSessionFactory
	{
		public static Session Make(int clientHashCode, Func<int, int, InGameRoom> roomFactory)
		{
			var session = new InGameSession(clientHashCode, roomFactory);
			if (session == null)
				return null;

			SessionManager.Instance.RegisterSession(session);
			return session;
		}
	}
}
