using System;
using System.Collections.Generic;

namespace ServerCore
{
	public class RoomManager : Singleton<RoomManager>
	{
		private Dictionary<int, Room> roomDictionary = new Dictionary<int, Room>();
		private object lockObj = new object();

		public Room Make(int roomId, int maxRoomMemberCount, Func<int, int, Room> roomFactory)
		{
			lock (lockObj)
			{
                if (roomDictionary.TryGetValue(roomId, out var room) == false)
                {
					room = roomFactory?.Invoke(roomId, maxRoomMemberCount);
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
