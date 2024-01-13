using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InGameServer
{
    internal class InGameRoom : Room
	{
		private Dictionary<int, int> playerCharacterDictionary = new Dictionary<int, int>();

        internal InGameRoom(int roomId, int maxSessionCount) : base(roomId, maxSessionCount)
		{
		}

        protected void OnEnter(Session session, REQ_ENTER_GAME packet)
        {
			if (roomSessions.Contains(session) == false)
				roomSessions.Add(session);

			playerCharacterDictionary[session.sessionId] = packet.characterType;

			var enter = new RES_BROADCAST_ENTER_GAME();
            enter.playerId = session.sessionId;

            Broadcast(enter.Write());
		}

		protected override void OnLeave(Session session)
		{
			if (roomSessions.Contains(session))
				roomSessions.Remove(session);

			if (playerCharacterDictionary.ContainsKey(session.sessionId))
				playerCharacterDictionary.Remove(session.sessionId);

			var leave = new RES_BROADCAST_LEAVE_GAME();
            leave.playerId = session.sessionId;

            Broadcast(leave.Write());
		}

		protected override void OnReady(Session session)
		{
			var connected = new RES_CONNECTED();
			session.Send(connected.Write());
		}

		public void ResponsePlayerList(Session session)
        {
            jobQueue.Push(() =>
            {
                OnResponsePlayerList(session);
			});
        }

        private void OnResponsePlayerList(Session session)
        {
			var players = new RES_PLAYER_LIST();

			foreach (InGameSession s in roomSessions)
			{
				if (playerCharacterDictionary.TryGetValue(s.sessionId, out var characterType) == false)
					continue;

				players.players.Add(new RES_PLAYER_LIST.Player()
				{
					isSelf = s == session,
					playerId = s.sessionId,
					characterType = characterType
				});
			}

			session.Send(players.Write());
		}

		public void Send(InGameSession session, REQ_TRANSFORM packet)
		{
			jobQueue.Push(() =>
			{
				SendTransform(session, packet);
			});
		}

		public void Send(InGameSession session, REQ_ANIMATOR packet)
		{
			jobQueue.Push(() =>
			{
				SendAnimator(session, packet);
			});
		}

		public void Enter(InGameSession session, REQ_ENTER_GAME packet)
		{
			jobQueue.Push(() =>
			{
				OnEnter(session, packet);
			});
		}

		private void SendAnimator(InGameSession session, REQ_ANIMATOR packet)
		{
			// 모두에게 알린다
			var move = new RES_ANIMATOR();

			move.playerId = session.sessionId;
			move.Grounded = packet.Grounded;
			move.speed = packet.speed;
			move.FreeFall = packet.FreeFall;
			move.Jump = packet.Jump;
			move.MotionSpeed = packet.MotionSpeed;

			Broadcast(move.Write());
		}

        private void SendTransform(InGameSession session, REQ_TRANSFORM packet)
        {
            var move = new RES_TRANSFORM();

            move.playerId = session.sessionId;
            move.posX = packet.posX;
            move.posY = packet.posY;
            move.posZ = packet.posZ;
			move.rotX = packet.rotX;
			move.rotY = packet.rotY;
			move.rotZ = packet.rotZ;

            Broadcast(move.Write());
        }
	}
}
