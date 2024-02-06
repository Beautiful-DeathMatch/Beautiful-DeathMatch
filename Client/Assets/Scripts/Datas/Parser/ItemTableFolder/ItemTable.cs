using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("ItemTable.asset")]
public partial class ItemTable : ScriptParser
{
	public override void Parser()
	{
		itemDataDictionary.Clear();
		foreach(var value in itemDataList)
		{
			itemDataDictionary.Add(value.key, value);
		}
		itemTypeDictionary.Clear();
		foreach(var value in itemTypeList)
		{
			itemTypeDictionary.Add(value.key, value);
		}
		itemPositionDictionary.Clear();
		foreach(var value in itemPositionList)
		{
			itemPositionDictionary.Add(value.key, value);
		}
		itemStartingPlayerDictionary.Clear();
		foreach(var value in itemStartingPlayerList)
		{
			itemStartingPlayerDictionary.Add(value.key, value);
		}
		boxDataDictionary.Clear();
		foreach(var value in boxDataList)
		{
			boxDataDictionary.Add(value.key, value);
		}
		boxStartingDictionary.Clear();
		foreach(var value in boxStartingList)
		{
			boxStartingDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class ItemData
	{
		public ENUM_ITEM_TYPE key;
		public int maxUsableCount;
		public int hpAmount;
		public int attackDistance;
		public string nameKey;
	}

	public List<ItemData> itemDataList = new List<ItemData>();
	[System.Serializable]
	public class ItemDataDictionary : SerializableDictionary<ENUM_ITEM_TYPE, ItemData> {}
	public ItemDataDictionary itemDataDictionary = new ItemDataDictionary();

	[Serializable]
	public class ItemType
	{
		public int key;
		public ENUM_ITEM_TYPE itemType;
	}

	public List<ItemType> itemTypeList = new List<ItemType>();
	[System.Serializable]
	public class ItemTypeDictionary : SerializableDictionary<int, ItemType> {}
	public ItemTypeDictionary itemTypeDictionary = new ItemTypeDictionary();

	[Serializable]
	public class ItemPosition
	{
		public int key;
		public float x;
		public float y;
		public float z;
	}

	public List<ItemPosition> itemPositionList = new List<ItemPosition>();
	[System.Serializable]
	public class ItemPositionDictionary : SerializableDictionary<int, ItemPosition> {}
	public ItemPositionDictionary itemPositionDictionary = new ItemPositionDictionary();

	[Serializable]
	public class ItemStartingPlayer
	{
		public int key;
		public int playerIndex;
	}

	public List<ItemStartingPlayer> itemStartingPlayerList = new List<ItemStartingPlayer>();
	[System.Serializable]
	public class ItemStartingPlayerDictionary : SerializableDictionary<int, ItemStartingPlayer> {}
	public ItemStartingPlayerDictionary itemStartingPlayerDictionary = new ItemStartingPlayerDictionary();

	[Serializable]
	public class BoxData
	{
		public int key;
		public int boxId;
	}

	public List<BoxData> boxDataList = new List<BoxData>();
	[System.Serializable]
	public class BoxDataDictionary : SerializableDictionary<int, BoxData> {}
	public BoxDataDictionary boxDataDictionary = new BoxDataDictionary();

	[Serializable]
	public class BoxStarting
	{
		public int key;
		public int areaIndex;
	}

	public List<BoxStarting> boxStartingList = new List<BoxStarting>();
	[System.Serializable]
	public class BoxStartingDictionary : SerializableDictionary<int, BoxStarting> {}
	public BoxStartingDictionary boxStartingDictionary = new BoxStartingDictionary();


}
