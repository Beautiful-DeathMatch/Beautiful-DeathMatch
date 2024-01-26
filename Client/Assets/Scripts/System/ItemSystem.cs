using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_ITEM_TYPE
{
	None = -1,
    Gun = 0,
	Knife = 1,
}

/// <summary>
/// 실제 사용 가능한 아이템
/// </summary>
public class ItemData
{
	public readonly int itemId;
	public readonly ItemTable.ItemData tableData;

	public int currentUsableCount { get; private set; }

	public ItemData(int itemId, ItemTable.ItemData tableData)
	{
		this.itemId = itemId;
		this.currentUsableCount = currentUsableCount;
		this.tableData = tableData;
	}

	public void UseItem()
	{
		currentUsableCount = currentUsableCount > 0 ? currentUsableCount - 1 : 0;
	}
}

public class PlayerItemSlot
{
	public int[] itemSlots = new int[5];

	public bool TryAddItem(int itemId, ENUM_ITEM_TYPE itemType)
	{
		switch(itemType)
		{
			case ENUM_ITEM_TYPE.Gun:
			case ENUM_ITEM_TYPE.Knife:

				int itemTypeInt = (int)itemType;

				if (itemSlots[itemTypeInt] != -1)
				{
					itemSlots[itemTypeInt] = itemId;
					return true;
				}
				else
				{
					return false;
				}

			default:

				for (int i = 2; i < itemSlots.Length; i++)
				{
					if (itemSlots[i] == -1)
					{
						itemSlots[i] = itemId;
						return true;
					}
				}

				return false;
		}
	}

	public int GetItemId(int slotIndex)
	{
		if (slotIndex >= itemSlots.Length)
			return -1;

		return itemSlots[slotIndex];
	}
}

public class ItemSystem : MonoSystem
{
	[System.Serializable]
	public class FieldItemDictionary : SerializableDictionary<ENUM_ITEM_TYPE, FieldItemComponent> { }
	[SerializeField] private FieldItemDictionary fieldItemPrefabDictionary = new FieldItemDictionary();

	[SerializeField] private ItemTable itemTable;

	/// <summary>
	/// 아래 둘을 동기화 할 예정입니다.
	/// 1. 살아있는 필드 아이템 목록
	/// 2. 유저가 획득하여 사용 중인 아이템의 현재 정보 
	/// </summary>
	private Dictionary<int, FieldItemComponent> fieldItemComponentDictionary = new Dictionary<int, FieldItemComponent>();
	private Dictionary<int, ItemData> dynamicItemDataDictionary = new Dictionary<int, ItemData>();

	private Dictionary<int, PlayerItemSlot> playerItemSlotDictionary = new Dictionary<int, PlayerItemSlot>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		fieldItemComponentDictionary.Clear();
		dynamicItemDataDictionary.Clear();

		foreach (var itemId in itemTable.GetAllItemIds())
		{
			var itemData = itemTable.GetItemData(itemId);
			if (itemData == null)
				continue;

			var dynamicItemData = new ItemData(itemId, itemData);
			dynamicItemDataDictionary.Add(itemId, dynamicItemData);

			var fieldItemObj = CreateFieldItem(itemId, itemData.key);
			fieldItemComponentDictionary.Add(itemId, fieldItemObj);
		}
	}

	private IEnumerable<int> GetItemIds()
	{
		for (int i = 0; i < 10; i++)
		{
			yield return i;
		}
	}

	private FieldItemComponent CreateFieldItem(int itemId, ENUM_ITEM_TYPE itemType)
	{
		if (fieldItemPrefabDictionary.TryGetValue(itemType, out var prefab))
		{
			var fieldItemObj = Instantiate(prefab, transform);
			fieldItemObj.transform.SetParent(transform, false);
			fieldItemObj.gameObject.SetActive(false);

			return fieldItemObj;
		}

		return null;
	}

	public ENUM_ITEM_TYPE GetItemType(int itemId)
	{
		var data = GetDynamicItemData(itemId);
		if (data == null)
			return ENUM_ITEM_TYPE.None;

		return data.tableData.key;
	}

	public ItemData GetDynamicItemData(int itemId)
	{
		if (dynamicItemDataDictionary.TryGetValue(itemId, out  var itemData))
		{
			return itemData;
		}

		return null;
	}

	public bool TryGiveItem(int playerId, int itemId, ENUM_ITEM_TYPE itemType)
	{
		if (playerItemSlotDictionary.TryGetValue(playerId, out var slot))
		{
			return slot.TryAddItem(itemId, itemType);
		}

		return false;
	}

	public int GetItemId(int playerId, int slotIndex)
	{
		if (playerItemSlotDictionary.TryGetValue(playerId, out var slot) == false)
			return -1;

		return slot.GetItemId(slotIndex);
	}

	public bool TryUseItem(int playerId, int slotIndex, Action<int, ItemData> onUseItem = null)
	{
		int itemId = GetItemId(playerId, slotIndex);
		if (itemId == -1)
			return false;

		var dynamicItemData = GetDynamicItemData(itemId);
		if (dynamicItemData == null)
			return false;

		if (dynamicItemData.tableData.maxUsableCount > 0 && dynamicItemData.currentUsableCount <= 0)
			return false;

		dynamicItemData.UseItem();
		onUseItem?.Invoke(itemId, dynamicItemData);
		return true;
	}
}
