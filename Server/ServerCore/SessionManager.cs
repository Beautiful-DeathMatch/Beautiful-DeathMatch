using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public class SessionManager : Singleton<SessionManager>
	{
		private readonly ConcurrentDictionary<int, Session> sessions = new ConcurrentDictionary<int, Session>();
		private Timer sessionCleanUpTimer = null;

		private const int SessionCheckIntervalSeconds = 2;

		protected override void OnAwakeInstance()
		{
			base.OnAwakeInstance();

			sessionCleanUpTimer = new Timer(CleanUpSessions, null, TimeSpan.Zero, TimeSpan.FromSeconds(SessionCheckIntervalSeconds));
		}

		protected override void OnDisposeInstance()
		{
			base.OnDisposeInstance();

			sessionCleanUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void CleanUpSessions(object state)
		{
			foreach(var session in sessions.Values)
			{
				if (IsValidSession(session.sessionId))
					continue;

				Console.WriteLine($"Clean Up Session : {session.sessionId} : {DateTime.Now}");
				session.Disconnect();
			}
		}

		public void RegisterSession(Session session)
		{
			if (session == null)
				return;

			if (sessions.ContainsKey(session.sessionId))
				return;

			if (sessions.TryAdd(session.sessionId, session) == false)
				return;

			Console.WriteLine($"Connected : {session.sessionId} : {DateTime.Now}");
		}

		public void UnRegisterSession(Session session)
		{
			if (session == null)
				return;

			if (sessions.ContainsKey(session.sessionId) == false)
				return;

			if (sessions.TryRemove(session.sessionId, out var dontRemoveSession) == false)
			{
				Console.WriteLine($"Can't Remove Session : {dontRemoveSession.sessionId}");
				return;
			}

			Console.WriteLine($"OnDisconnected : {session.sessionId} : {DateTime.Now}");

		}

		public bool IsValidSession(int sessionId)
		{
			if (sessions.TryGetValue(sessionId, out var session) == false)
				return false;

			return session.IsValid();
		}
	}
}
