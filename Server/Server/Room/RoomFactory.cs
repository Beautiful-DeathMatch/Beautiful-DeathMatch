using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	public enum RoomType
	{
		InGame = 0,
	}

	public class RoomFactory : Singleton<RoomFactory>
	{
		private int currentRoomId = 0; // 해시값을 추출하는 함수가 필요하다.

		private Dictionary<int, Room> roomDictionary = new Dictionary<int, Room>();
		private object lockObj = new object();

		private const int maxRoomMemberCount = 8;

		public Room Make(RoomType roomType)
		{
			lock (lockObj)
			{
				var room = MakeInternal(roomType);
				roomDictionary[room.roomId] = room;
				return room;
			}
		}

		private Room MakeInternal(RoomType type)
		{
			switch (type)
			{
				case RoomType.InGame:
					return new InGameRoom(++currentRoomId, maxRoomMemberCount);
			}

			return null;
		}

		public void Remove(InGameRoom room)
		{
			lock (lockObj)
			{
				roomDictionary.Remove(room.roomId);
			}
		}
	}
}
