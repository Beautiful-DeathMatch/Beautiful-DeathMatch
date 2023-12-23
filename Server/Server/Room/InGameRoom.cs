using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class InGameRoom : Room
	{

		public InGameRoom(int roomId, int maxSessionCount) : base(roomId, maxSessionCount)
		{
		}

        protected override void OnEnter(Session session)
        {
            // 플레이어 추가하고
            roomSessions.Add(session);

			// 신입생 입장을 모두에게 알린다
			var enter = new RES_BROADCAST_ENTER_GAME();
            enter.playerId = session.sessionId;

            Broadcast(enter.Write());
		}

		protected override void OnLeave(Session session)
        {
            // 플레이어 제거하고
            roomSessions.Remove(session);

            // 모두에게 알린다
            var leave = new RES_BROADCAST_LEAVE_GAME();
            leave.playerId = session.sessionId;

            Broadcast(leave.Write());
		}

		public void BroadcastPlayerList(Session session)
        {
            jobQueue.Push(() =>
            {
                OnBroadcastPlayerList(session);
			});
        }

        private void OnBroadcastPlayerList(Session session)
        {
			var players = new RES_PLAYER_LIST();

			foreach (IngameSession s in roomSessions)
			{
				players.players.Add(new RES_PLAYER_LIST.Player()
				{
					isSelf = s == session,
					playerId = s.sessionId
				});
			}

			session.Send(players.Write());
		}

		public void Send(IngameSession session, REQ_TRANSFORM packet)
		{
			jobQueue.Push(() =>
			{
				SendTransform(session, packet);
			});
		}

		public void Send(IngameSession session, REQ_ANIMATOR packet)
		{
			jobQueue.Push(() =>
			{
				SendAnimator(session, packet);
			});
		}

		private void SendAnimator(IngameSession session, REQ_ANIMATOR packet)
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

        private void SendTransform(IngameSession session, REQ_TRANSFORM packet)
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
