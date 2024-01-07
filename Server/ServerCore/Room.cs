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

        protected JobQueue jobQueue = new JobQueue();
        protected List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        private int maxSessionCount = 0;


		public Room(int roomId, int maxSessionCount)
		{
			this.roomId = roomId;
			this.maxSessionCount = maxSessionCount;
		}

		public void Flush()
		{
            jobQueue.Push(OnFlush);
        }

        private void OnFlush()
        {
            foreach (var s in roomSessions)
            {
                s.Send(pendingList);
            }

            pendingList.Clear();
        }

        protected void Broadcast(ArraySegment<byte> segment)
        {
            pendingList.Add(segment);
        }

        public void Ready(Session session)
        {
			jobQueue.Push(() =>
			{
				OnReady(session);
			});
		}

		protected virtual void OnReady(Session session)
        {
            
        }

        public void Leave(Session session)
        {
            jobQueue.Push(() =>
            {
                OnLeave(session);
			});
        }

        protected virtual void OnLeave(Session session)
        {
        }
	}
}
