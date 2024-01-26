using System;
using System.Collections.Generic;

public partial class ItemTable
{
	public IEnumerable<int> GetAllItemIds()
	{
		return itemTypeDictionary.Keys;
	}

	public ItemData GetItemData(ENUM_ITEM_TYPE itemType)
	{
		if (itemDataDictionary.TryGetValue(itemType, out var data))
		{
			return data;
		}

		return null;
	}

	public ENUM_ITEM_TYPE GetItemType(int itemId)
	{
		if (itemTypeDictionary.TryGetValue(itemId, out var data))
		{
			return data.itemType;
		}

		return ENUM_ITEM_TYPE.None;
	}

	public ItemData GetItemData(int itemId)
	{
		var type = GetItemType(itemId);
		return GetItemData(type);
	}
}