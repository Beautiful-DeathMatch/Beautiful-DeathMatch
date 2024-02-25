using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemComponent : MonoComponent<ItemSystem>
{
	[SerializeField] private CharacterViewComponent characterViewComponent = null;
	[SerializeField] private PlayerAttackComponent attackComponent = null;
	[SerializeField] private ThirdPersonController controller = null;

	[SerializeField] private Animator animator;

	private string[] UseStateNames = new string[] { "KnifeUse", "GunUse", "SmokeUse", "ButtonUse"};
	private int upperLayerIndex = 1;

	private int ItemTypeHash = Animator.StringToHash("ItemType");
	private int ItemUseTriggerHash = Animator.StringToHash("ItemUseTrigger");
	private int ItemChangeTriggerHash = Animator.StringToHash("ItemChangeTrigger");

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
		
		int slotItemId = System.GetItemId(playerId, slotIndex);
		if (slotItemId == -1)
			return;

		// 교체가 가능하다면 설정함
		currentItemSlotIndex = slotIndex;

		var itemType = System.GetItemType(slotItemId);
		characterViewComponent.TryAttachRightHand(itemType);

		if (animator.GetBool(ItemChangeTriggerHash) == false)
		{
			animator.SetTrigger(ItemChangeTriggerHash);
		}

		animator.SetInteger(ItemTypeHash, (int)itemType);
	}

	private void OnClickUse()
	{
		if (animator.IsCurrentState(upperLayerIndex, UseStateNames))
			return;

		if (animator.GetBool(ItemUseTriggerHash))
			return;

		System.TryUseItem(playerId, currentItemSlotIndex, OnUseItem);
	}

	private void OnUseItem(int itemId, DynamicItemData usedItemData)
	{
		var itemType = usedItemData.tableData.key;
		switch (itemType)
		{
			case ENUM_ITEM_TYPE.Knife:
				attackComponent.TrySlashAttack(usedItemData);
				break;

			case ENUM_ITEM_TYPE.Gun:
				attackComponent.TryShotAttack(usedItemData);
				break;
		}

		animator.SetTrigger(ItemUseTriggerHash);
	}
}
