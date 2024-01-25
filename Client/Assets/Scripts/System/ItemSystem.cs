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

public class ItemData
{	
	public readonly ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public readonly int maxUsableCount = 0;

	public readonly int hpAmount = 0;
	public readonly int attackDistance = 0;

	public ItemData(ENUM_ITEM_TYPE itemType, int maxUsableCount, int hpAmount, int attackDistance)
	{
		this.itemType = itemType;
		this.maxUsableCount = maxUsableCount;
		this.hpAmount = hpAmount;
		this.attackDistance = attackDistance;
	}
}

/// <summary>
/// 실제 사용 가능한 아이템
/// </summary>
public class DynamicItemData : ItemData
{
	public readonly int itemId;

	public int currentUsableCount { get; private set; }

	public DynamicItemData(int itemId, ItemData itemData) : base(itemData.itemType, itemData.maxUsableCount, itemData.hpAmount, itemData.attackDistance)
	{
		this.itemId = itemId;
		this.currentUsableCount = currentUsableCount;
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

	private Dictionary<ENUM_ITEM_TYPE, ItemData> itemDataDictionary = new Dictionary<ENUM_ITEM_TYPE, ItemData>();
	private Dictionary<int, ENUM_ITEM_TYPE> itemTypeDictionary = new Dictionary<int, ENUM_ITEM_TYPE>();

	/// <summary>
	/// 아래 둘을 동기화 할 예정입니다.
	/// 1. 살아있는 필드 아이템 목록
	/// 2. 유저가 획득하여 사용 중인 아이템의 현재 정보 
	/// </summary>
	private Dictionary<int, FieldItemComponent> fieldItemComponentDictionary = new Dictionary<int, FieldItemComponent>();
	private Dictionary<int, DynamicItemData> dynamicItemDataDictionary = new Dictionary<int, DynamicItemData>();

	private Dictionary<int, PlayerItemSlot> playerItemSlotDictionary = new Dictionary<int, PlayerItemSlot>();

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		fieldItemComponentDictionary.Clear();
		dynamicItemDataDictionary.Clear();

		MakeDummyItemDataDictionary();
		MakeDummyItemTypeDictionary();

		foreach (var itemId in GetItemIds())
		{
			if (itemTypeDictionary.TryGetValue(itemId, out var itemType))
			{
				if (itemDataDictionary.TryGetValue(itemType, out var itemData))
				{
					var dynamicItemData = new DynamicItemData(itemId, itemData);
					dynamicItemDataDictionary.Add(itemId, dynamicItemData);
				}

				var fieldItemObj = CreateFieldItem(itemId, itemType);
				fieldItemComponentDictionary.Add(itemId, fieldItemObj);
			}
		}
	}

	private IEnumerable<int> GetItemIds()
	{
		for (int i = 0; i < 10; i++)
		{
			yield return i;
		}
	}

	private void MakeDummyItemTypeDictionary()
	{
		itemTypeDictionary = new Dictionary<int, ENUM_ITEM_TYPE>()
		{
			{ 0, ENUM_ITEM_TYPE.Knife },
			{ 1, ENUM_ITEM_TYPE.Knife },
			{ 2, ENUM_ITEM_TYPE.Gun },
			{ 3, ENUM_ITEM_TYPE.Gun },
			{ 4, ENUM_ITEM_TYPE.Knife },
			{ 5, ENUM_ITEM_TYPE.Gun },
			{ 6, ENUM_ITEM_TYPE.Knife },
			{ 7, ENUM_ITEM_TYPE.Gun },
			{ 8, ENUM_ITEM_TYPE.Knife },
			{ 9, ENUM_ITEM_TYPE.Knife },
		};
	}

	private void MakeDummyItemDataDictionary()
	{
		itemDataDictionary = new Dictionary<ENUM_ITEM_TYPE, ItemData>()
		{
			{ ENUM_ITEM_TYPE.Knife, new ItemData(ENUM_ITEM_TYPE.Knife, -1, 10, 10) },
			{ ENUM_ITEM_TYPE.Gun, new ItemData(ENUM_ITEM_TYPE.Gun, 5, 15, 100) },
		};
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

		return data.itemType;
	}

	public DynamicItemData GetDynamicItemData(int itemId)
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

	public bool TryUseItem(int playerId, int slotIndex, Action<int, DynamicItemData> onUseItem = null)
	{
		int itemId = GetItemId(playerId, slotIndex);
		if (itemId == -1)
			return false;

		var itemData = GetDynamicItemData(itemId);
		if (itemData == null)
			return false;

		if (itemData.maxUsableCount > 0 && itemData.currentUsableCount <= 0)
			return false;

		itemData.UseItem();
		onUseItem?.Invoke(itemId, itemData);
		return true;
	}
}
