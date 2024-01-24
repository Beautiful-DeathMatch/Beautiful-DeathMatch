using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemComponent : MonoComponent<ItemSystem>
{
    [SerializeField] private PlayerInteractionComponent interactionComponent = null;
	[SerializeField] private CharacterViewComponent characterViewComponent = null;
	
	[SerializeField] private PlayerAttackComponent attackComponent = null;

	[SerializeField] private ThirdPersonController controller = null;

	private int playerId = -1;

	private const int MaxItemSlotCount = 5;
	private int[] itemSlots = new int[MaxItemSlotCount]
	{
		-1, -1, -1, -1, -1
	};

	public int currentItemSlotIndex { get; private set; }

	private void OnEnable()
	{
		interactionComponent.onPressInteract += OnPressInteract;
		
		controller.onClickNumber += OnClickNumber;
		controller.onClickUse += OnClickUse;
	}

	private void OnDisable()
	{
		interactionComponent.onPressInteract -= OnPressInteract;

		controller.onClickNumber -= OnClickNumber;
		controller.onClickUse -= OnClickUse;
	}
	
	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	private void OnPressInteract(IInteractable interactableObject)
	{
		if (interactableObject == null)
			return;

		if (interactableObject.TryInteract(playerId) == false)
		{
			Debug.LogError("현재 상호 작용이 불가능한 오브젝트입니다.");
			return;
		}

		interactableObject.EndInteract();
	}

	private void OnClickNumber(int number)
	{
		int slotIndex = number - 1;
		if (slotIndex >= itemSlots.Length)
			return;

		int slotItemId = itemSlots[slotIndex];
		if (slotItemId == -1)
			return;

		// 교체가 가능하다면 설정함
		currentItemSlotIndex = slotIndex;

		var itemType = System.GetItemType(slotItemId);
		characterViewComponent.TryAttachRightHand(itemType);
	}

	private void OnClickUse()
	{
		if (currentItemSlotIndex >= itemSlots.Length)
			return;

		int slotItemId = itemSlots[currentItemSlotIndex];
		if (slotItemId == -1)
			return;

		System.TryUseItem(slotItemId, OnUseItem);
	}

	private void OnUseItem(int itemId, DynamicItemData usedItemData)
	{
		switch(usedItemData.itemType)
		{
			case ENUM_ITEM_TYPE.Knife:
			case ENUM_ITEM_TYPE.Gun:
				attackComponent.Attack(usedItemData);
				break;
		}
	}
}
