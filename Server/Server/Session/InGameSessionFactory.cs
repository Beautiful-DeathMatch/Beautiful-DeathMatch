using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{

	public class InGameSessionFactory
	{
		private static object lockObj = new object();

		public static Session Make(int clientHashCode, Func<int, int, InGameRoom> roomFactory)
		{
			lock (lockObj)
			{
				var session = new InGameSession(clientHashCode, roomFactory);
				if (session == null)
					return null;

				InGameSessionManager.Instance.RegisterSession(session);
				return session;
			}
		}

		public static void Remove(Session session)
		{
			lock (lockObj)
			{
				InGameSessionManager.Instance.UnRegisterSession(session);
			}
		}
	}
}
