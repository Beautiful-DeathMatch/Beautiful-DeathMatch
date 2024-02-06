using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

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

	public int GetItemStartingPlayer(int itemId)
	{
		if (itemStartingPlayerDictionary.TryGetValue(itemId, out var data))
		{
			return data.playerIndex;
		}
		else
			return -1;
	}

	public UnityEngine.Vector3 GetItemPosition(int itemId)
	{
		if (itemPositionDictionary.TryGetValue(itemId, out var data))
		{
			return new UnityEngine.Vector3(data.x,data.y,data.z);
		}
		else
			return new UnityEngine.Vector3(0f,0f,0f);
	}

	public IEnumerable<int> GetAllBoxIds()
	{
		return boxStartingDictionary.Keys;
	}

	public List<int> GetItemListInBox(int boxId)
	{
		List<int> itemList = new();
		foreach(var data in boxDataDictionary.Values)
		{
			if(data.boxId == boxId)
			{
				itemList.Add(data.key);
			}
		}
		return itemList;
	}

	public int GetBoxSpawnAreaIndex(int boxId)
	{
		if (boxStartingDictionary.TryGetValue(boxId, out var data))
		{
			return data.areaIndex;
		}

		return -1;
	}

}