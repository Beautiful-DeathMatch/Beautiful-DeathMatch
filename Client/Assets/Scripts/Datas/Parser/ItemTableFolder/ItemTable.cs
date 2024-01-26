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
	}

	[Serializable]
	public class ItemData
	{
		public ENUM_ITEM_TYPE key;
		public int maxUsableCount;
		public int hpAmount;
		public int attackDistance;
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


}
