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

		object lockObj = new object();

		public Session Make(SessionType type)
		{
			lock (lockObj)
			{
				var session = MakeInternal(type);
				if (session == null)
					return null;

				SessionManager.Instance.RegisterSession(session);
				return session;
			}
		}

		private Session MakeInternal(SessionType type)
		{
			switch (type)
			{
				case SessionType.InGame:
					return new IngameSession(++currentSessionId);
			}

			return null;
		}

		public void Remove(Session session)
		{
			lock (lockObj)
			{
				SessionManager.Instance.UnRegisterSession(session);
			}
		}
	}
}
