using Mono.CecilX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using static ItemTable;
using static UnityEditor.Progress;

public enum ENUM_ITEM_TYPE
{
	None = -1,
    Gun = 0,
	Knife = 1,
	Armor,
	Smoke,
	Rader,
	Teleport,
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
		this.currentUsableCount = tableData.maxUsableCount;
		this.tableData = tableData;
	}

	public void UseItem()
	{
		currentUsableCount = currentUsableCount > 0 ? currentUsableCount - 1 : 0;
	}
}

public class PlayerItemSlot
{
	public readonly int[] itemSlots = new int[5] // 각 슬롯의 item ID들
	{
		-1, -1, -1, -1, -1
	};

	public bool TryAddItem(int itemId, ENUM_ITEM_TYPE itemType)
	{
		switch (itemType)
		{
			case ENUM_ITEM_TYPE.Gun:
			case ENUM_ITEM_TYPE.Knife:

				int itemTypeInt = (int)itemType;

				if (itemSlots[itemTypeInt] == -1)
				{
					itemSlots[itemTypeInt] = itemId;
					return true;
				}
				else
				{
					return false;
				}

			// Gun, Knife 가 아니면 2번 슬롯부터 채우기
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

    [System.Serializable]
    public class ItemSpriteDictionary : SerializableDictionary<ENUM_ITEM_TYPE, Sprite> { }
    [SerializeField] private ItemSpriteDictionary itemSpriteDictionary = new ItemSpriteDictionary();

    [SerializeField] private ItemTable itemTable;

	/// <summary>
	/// 아래 목록을 동기화 할 예정입니다.
	/// 1. 관리 중인 필드 아이템 목록 : itemId - 필드 아이템
	/// 2. 모든 아이템들의 현재 상태 : itemId - 아이템
	/// 3. 유저가 보유한 Item Slot 정보 : playerId - 슬롯 정보
	/// </summary>
	private Dictionary<int, FieldItemComponent> fieldItemComponentDictionary = new Dictionary<int, FieldItemComponent>();
	private Dictionary<int, ItemData> itemDataDictionary = new Dictionary<int, ItemData>();
	private Dictionary<int, PlayerItemSlot> playerItemSlotDictionary = new Dictionary<int, PlayerItemSlot>();


	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		base.OnEnter(sceneModuleParam);

		List<int> playerIdList = new();

		if (sceneModuleParam is BattleSceneModule.Param battleParam)
		{
			playerItemSlotDictionary.Clear();
			playerIdList.Clear();

            foreach (var playerInfo in battleParam.playerInfoList)
			{
				playerItemSlotDictionary.Add(playerInfo.playerId, new PlayerItemSlot());
				playerIdList.Add(playerInfo.playerId);
            }
        }

        fieldItemComponentDictionary.Clear();
		itemDataDictionary.Clear();

		// itemTable 안에 있는 모든 아이템 생성 (Dictionary 추가, Object 추가)
		foreach (var itemId in itemTable.GetAllItemIds())
		{
			var itemData = itemTable.GetItemData(itemId);
			if (itemData == null)
				continue;

			var dynamicItemData = new ItemData(itemId, itemData);
			itemDataDictionary.Add(itemId, dynamicItemData);			// DataDictionary

			var fieldItemObj = CreateFieldItem(itemId, itemData.key);	// Instantiate
			fieldItemComponentDictionary.Add(itemId, fieldItemObj);     // FieldDictionary

			var startingPlayerIndex = itemTable.GetItemStartingPlayer(itemId);
			if(startingPlayerIndex >= 0 && startingPlayerIndex < playerIdList.Count)
				GivePlayerStartingItem(playerIdList[startingPlayerIndex], itemId);

		}

		// 테스트용
		SpawnFieldItem(101);
		SpawnFieldItem(111);
    }

	// ID 조회해서 있으면 오브젝트 SetActive
	private void SpawnFieldItem(int itemId)
	{
		if (fieldItemComponentDictionary.TryGetValue(itemId, out var fieldItemObj))
		{
            fieldItemObj.gameObject.SetActive(true);
        }
    }

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

		// 순서 등에 따른 스폰 로직
    }

	// 특정 itemId로 itemType의 아이템 생성 (Instantiate)
    private FieldItemComponent CreateFieldItem(int itemId, ENUM_ITEM_TYPE itemType)
	{
		if (fieldItemPrefabDictionary.TryGetValue(itemType, out var prefab))
		{
			var fieldItemObj = Instantiate(prefab, transform);
			fieldItemObj.SetItemId(itemId);

			fieldItemObj.transform.SetParent(transform, true);
			fieldItemObj.transform.localPosition = itemTable.GetItemPosition(itemId);

            fieldItemObj.gameObject.SetActive(false);

			return fieldItemObj;
		}
		else if (fieldItemPrefabDictionary.TryGetValue(ENUM_ITEM_TYPE.None, out var noneFieldPrefab))
		{
			var fieldItemObj = Instantiate(noneFieldPrefab, transform);
			fieldItemObj.SetItemId(itemId);
			fieldItemObj.SetItemType(itemType);

			fieldItemObj.transform.SetParent(transform, true);
			fieldItemObj.transform.localPosition = itemTable.GetItemPosition(itemId);

            fieldItemObj.gameObject.SetActive(false);

			return fieldItemObj;
		}
			return null;
	}

	// 스타팅 플레이어가 있는 경우 아이템 제공, 필드에선 삭제
	private void GivePlayerStartingItem(int playerId, int itemId)
	{
		var itemType = itemTable.GetItemType(itemId);
		if (TryGiveItem(playerId, itemId, itemType) == false)
		{
			Debug.LogError($"플레이어 {playerId}가 {itemId}, {itemType} 획득에 실패");
		}
		else
        {
            Debug.Log($"{playerId}와 {itemId} : {itemType}가 시작 아이템 제공에 성공하였습니다.");
			TryDestroyFieldItem(itemId);
        }
	}

	// 특정 플레이어가 가지고 있는 아이템 스프라이트들 출력 (UI용)
	public IEnumerable<Sprite> GetPlayerItemSprites(int playerId)
	{
		return GetPlayerItemTypes(playerId).Select(type =>
		{
			if (itemSpriteDictionary.TryGetValue(type, out var sprite))
			{
				return sprite;
			}
			return null;
		});
	}

	// 특정 플레이어가 가지고 있는 아이템 타입들 출력 (UI용)
	public IEnumerable<ENUM_ITEM_TYPE> GetPlayerItemTypes(int playerId)
	{
        if (playerItemSlotDictionary.TryGetValue(playerId, out var slot))
        {
            foreach (int itemId in slot.itemSlots)
			{
				yield return GetItemType(itemId);
			}
        }
    }

	// 특정 플레이어가 가지고 있는 Item Data 출력
	public ItemData GetPlayerItemData(int playerId, int slotIndex)
	{
		int itemId = GetItemId(playerId, slotIndex);
		return GetItemData(itemId);
	}

	// item ID로 Item Type 찾기 
	public ENUM_ITEM_TYPE GetItemType(int itemId)
	{
		var data = GetItemData(itemId);
		if (data == null)
			return ENUM_ITEM_TYPE.None;

		return data.tableData.key;
	}

	// item ID로 Item Data 찾기 
	public ItemData GetItemData(int itemId)
	{
		if (itemDataDictionary.TryGetValue(itemId, out  var itemData))
		{
			return itemData;
		}

		return null;
	}

	// Player(ID) 에게 Item(ID) 으로 Item(Type) 주기 (slotDictionary 에 추가만)
	public bool TryGiveItem(int playerId, int itemId, ENUM_ITEM_TYPE itemType)
	{
		if (playerItemSlotDictionary.TryGetValue(playerId, out var slot))
		{
			return slot.TryAddItem(itemId, itemType);
		}

		return false;
	}

	// Player(ID) 의 Slot(Index) 에 있는 Item ID 가져오기
	public int GetItemId(int playerId, int slotIndex)
	{
		if (playerItemSlotDictionary.TryGetValue(playerId, out var slot) == false)
			return -1;

		return slot.GetItemId(slotIndex);
	}

	// FieldItem(ID) 필드에서만 삭제 (Object Destroy 및 Field Dictionary 삭제)
	public bool TryDestroyFieldItem(int itemId)
	{
		if (fieldItemComponentDictionary.TryGetValue(itemId, out var fieldItem) == false)
			return false;

		Destroy(fieldItem.gameObject);
		fieldItemComponentDictionary.Remove(itemId);

		return true;
	}

	// Player(ID)의 Slot(Index)에 있는 아이템 사용 횟수 차감
	public bool TryUseItem(int playerId, int slotIndex, Action<int, ItemData> onUseItem = null)
	{
		int itemId = GetItemId(playerId, slotIndex);
		if (itemId == -1)
			return false;

		var itemData = GetItemData(itemId);
		if (itemData == null)
			return false;

		if (itemData.tableData.maxUsableCount > 0 && itemData.currentUsableCount <= 0)
			return false;

		itemData.UseItem();
		onUseItem?.Invoke(itemId, itemData);
		return true;
	}
}
