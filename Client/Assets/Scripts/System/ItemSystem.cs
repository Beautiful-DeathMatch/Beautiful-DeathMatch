using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_ITEM_TYPE
{
	None = -1,
    Knife,
    Gun,
}

public class ItemData
{	
	public readonly ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public readonly int maxUsableCount = 0;

	public readonly int hpAmount = 0;

	public ItemData(ENUM_ITEM_TYPE itemType, int maxUsableCount, int hpAmount)
	{
		this.itemType = itemType;
		this.maxUsableCount = maxUsableCount;
		this.hpAmount = hpAmount;
	}
}

/// <summary>
/// 실제 사용 가능한 아이템
/// </summary>
public class DynamicItemData : ItemData
{
	public readonly int itemId;

	public int currentUsableCount { get; private set; }

	public DynamicItemData(int itemId, ItemData itemData) : base(itemData.itemType, itemData.maxUsableCount, itemData.hpAmount)
	{
		this.itemId = itemId;
		this.currentUsableCount = currentUsableCount;
	}

	public DynamicItemData(int itemId, ENUM_ITEM_TYPE itemType, int maxUsableCount, int hpAmount) : base(itemType, maxUsableCount, hpAmount)
	{
		this.itemId = itemId;
	}

	public void UseItem()
	{
		currentUsableCount = currentUsableCount > 0 ? currentUsableCount - 1 : 0;
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

		fieldItemComponentDictionary[0].gameObject.SetActive(true);
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
			{ ENUM_ITEM_TYPE.Knife, new ItemData(ENUM_ITEM_TYPE.Knife, -1, 10) },
			{ ENUM_ITEM_TYPE.Gun, new ItemData(ENUM_ITEM_TYPE.Gun, 5, 15) },
		};
	}

	private FieldItemComponent CreateFieldItem(int itemId, ENUM_ITEM_TYPE itemType)
	{
		if (fieldItemPrefabDictionary.TryGetValue(itemType, out var prefab))
		{
			var fieldItemObj = Instantiate(prefab, transform);
			fieldItemObj.transform.SetParent(transform, false);
			fieldItemObj.gameObject.SetActive(false);

			fieldItemObj.Initialize(itemId);
			fieldItemObj.SetItemType(itemType);

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

	public bool TryUseItem(int itemId, Action<int, DynamicItemData> onUseItem = null)
	{
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
