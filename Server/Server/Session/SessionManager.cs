﻿using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
	internal class SessionManager : Singleton<SessionManager>
	{
		private readonly ConcurrentDictionary<int, Session> sessions = new ConcurrentDictionary<int, Session>();

		private const int SessionTimeOutSeconds = 10;
		private const int SessionCheckIntervalSeconds = 2;

		private Timer sessionCleanUpTimer = null;

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

			TimeSpan delta = DateTime.Now - session.LastActivityTime;

			return delta.TotalSeconds <= SessionTimeOutSeconds;
		}
	}
}
