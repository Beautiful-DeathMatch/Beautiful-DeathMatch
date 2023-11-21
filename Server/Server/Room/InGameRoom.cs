using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class InGameRoom : Room
	{
        private JobQueue jobQueue = new JobQueue();
        private List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

		public InGameRoom(int roomId, int maxSessionCount) : base(roomId, maxSessionCount)
		{
		}

		public override void Flush()
        {
            jobQueue.Push(OnFlush);
        }

		public override void Enter(Session session)
		{
			jobQueue.Push(() =>
			{
				OnEnter(session as ClientSession);
			});
		}

		public override void Leave(Session session)
		{
			jobQueue.Push(() =>
			{
				OnLeave(session as ClientSession);
			});
		}

		public void Move(ClientSession session, C_Move packet)
		{
			jobQueue.Push(() =>
			{
				OnMove(session, packet);
			});
		}

		private void OnFlush()
        {
            foreach (var s in roomSessions)
            {
                s.Send(pendingList);
            }

            pendingList.Clear();
        }

		private void Broadcast(ArraySegment<byte> segment)
		{
			pendingList.Add(segment);
		}

		private void OnEnter(ClientSession session)
        {
            // 플레이어 추가하고
            roomSessions.Add(session);

            // 신입생한테 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in roomSessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = s == session,
                    playerId = s.sessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }
            session.Send(players.Write());

            // 신입생 입장을 모두에게 알린다
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.sessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;

            Broadcast(enter.Write());
        }

        private void OnLeave(ClientSession session)
        {
            // 플레이어 제거하고
            roomSessions.Remove(session);

            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.sessionId;

            Broadcast(leave.Write());
        }

        private void OnMove(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알린다
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.sessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            Broadcast(move.Write());
        }
	}
}
