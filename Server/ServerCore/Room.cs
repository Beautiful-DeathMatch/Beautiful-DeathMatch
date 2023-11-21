using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerCore
{
	public abstract class Room
	{
		public readonly int roomId = 0;

		public bool IsFull => roomSessions.Count > maxSessionCount;
		public bool IsEmpty => roomSessions.Any() == false;


		protected List<Session> roomSessions = new List<Session>();

		private int maxSessionCount = 0;


		public Room(int roomId, int maxSessionCount)
		{
			this.roomId = roomId;
			this.maxSessionCount = maxSessionCount;
		}

		public abstract void Flush();

		public abstract void Enter(Session session);

		public abstract void Leave(Session session);
	}
}
