using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemComponent : MonoComponent<ItemSystem>
{
	[SerializeField] private CharacterViewComponent characterViewComponent = null;
	
	[SerializeField] private PlayerAttackComponent attackComponent = null;

	[SerializeField] private ThirdPersonController controller = null;

	private int playerId = -1;

	public int currentItemSlotIndex { get; private set; } = 0;

	private void OnEnable()
	{		
		controller.onClickNumber += OnClickNumber;
		controller.onClickUse += OnClickUse;
	}

	private void OnDisable()
	{
		controller.onClickNumber -= OnClickNumber;
		controller.onClickUse -= OnClickUse;
	}
	
	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	private void OnClickNumber(int number)
	{
		int slotIndex = number - 1;
		
		Debug.Log(number);
		int slotItemId = System.GetItemId(playerId, slotIndex);
		if (slotItemId == -1)
			return;
		Debug.Log(slotItemId);
		// 교체가 가능하다면 설정함
		currentItemSlotIndex = slotIndex;

		var itemType = System.GetItemType(slotItemId);
		characterViewComponent.TryAttachRightHand(itemType);
	}

	private void OnClickUse()
	{
		System.TryUseItem(playerId, currentItemSlotIndex, OnUseItem);
	}

	private void OnUseItem(int itemId, ItemData usedItemData)
	{
		switch(usedItemData.tableData.key)
		{
			case ENUM_ITEM_TYPE.Knife:
			case ENUM_ITEM_TYPE.Gun:
				attackComponent.Attack(usedItemData);
				break;
		}
	}
}
