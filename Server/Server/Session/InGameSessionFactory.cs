using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{

	public class InGameSessionFactory
	{
		public static Session Make(int clientHashCode, Func<int, int, InGameRoom> roomFactory)
		{
			var session = new InGameSession(clientHashCode, roomFactory);
			if (session == null)
				return null;

			InGameSessionManager.Instance.RegisterSession(session);
			return session;
		}
	}
}
