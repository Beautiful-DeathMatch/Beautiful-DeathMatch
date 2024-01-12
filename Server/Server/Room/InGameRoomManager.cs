using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	public class InGameRoomManager : Singleton<InGameRoomManager>
	{
		private Dictionary<int, InGameRoom> roomDictionary = new Dictionary<int, InGameRoom>();
		private object lockObj = new object();

		public InGameRoom Make(int roomId, int maxRoomMemberCount)
		{
			lock (lockObj)
			{
                if (roomDictionary.TryGetValue(roomId, out var room) == false)
                {
					room = new InGameRoom(roomId, maxRoomMemberCount);
					roomDictionary.Add(roomId, room);
				}

				return room;
			}
		}

		private void Remove(int roomId)
		{
			lock (lockObj)
			{
				if (roomDictionary.ContainsKey(roomId))
				{
					roomDictionary.Remove(roomId);	
				}
			}
		}

		public void Flush()
		{
			List<int> removeRoomIds = new List<int>();

			foreach (var room in roomDictionary.Values)
			{
				if (room.IsEmpty == false)
				{
					room.Flush();
				}
				else
				{
					removeRoomIds.Add(room.roomId);
				}
			}

			foreach (var roomId in removeRoomIds)
			{
				Remove(roomId);
			}
		}
	}
}
