using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	public enum SessionType
	{
		InGame = 0,

	}

	public class SessionFactory : Singleton<SessionFactory>
	{
		int currentSessionId = 0; // 해시값을 추출하는 함수가 필요하다.

		Dictionary<int, Session> sessions = new Dictionary<int, Session>();
		object lockObj = new object();

		public Session Make(SessionType type)
		{
			lock (lockObj)
			{
				var session = MakeInternal(type);
				if (session == null)
					return null;

				sessions[session.sessionId] = session;
				Console.WriteLine($"Connected : {session.sessionId}");

				return session;
			}
		}

		private Session MakeInternal(SessionType type)
		{
			switch (type)
			{
				case SessionType.InGame:
					return new ClientSession(++currentSessionId);
			}

			return null;
		}

		public void Remove(ClientSession session)
		{
			lock (lockObj)
			{
				sessions.Remove(session.sessionId);
			}
		}
	}
}
