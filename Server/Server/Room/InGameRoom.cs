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
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;

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
					playerId = s.sessionId,
					posX = s.PosX,
					posY = s.PosY,
					posZ = s.PosZ,
				});
			}
			session.Send(players.Write());
		}

        public void Move(IngameSession session, REQ_MOVE packet)
		{
			jobQueue.Push(() =>
			{
				OnMove(session, packet);
			});
		}

        private void OnMove(IngameSession session, REQ_MOVE packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알린다
            var move = new RES_MOVE();
            move.playerId = session.sessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            Broadcast(move.Write());
        }
	}
}
