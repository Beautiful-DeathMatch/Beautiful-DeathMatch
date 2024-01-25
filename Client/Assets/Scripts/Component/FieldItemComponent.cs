using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_INTERACT_TYPE
{
	NORMAL = 0,
	ITEM = 1,
}


public interface IInteractable
{
	ENUM_INTERACT_TYPE Type { get; }

	bool IsInteractable();
	bool TryStartInteract(int playerId);
	void SuccessInteract(int playerId);
    void EndInteract();
}

public class FieldItemComponent : MonoComponent<ItemSystem>, IInteractable
{
	[SerializeField] private ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.ITEM;

	private int itemId = -1;

	public void SetItemId(int itemId)
	{
		this.itemId = itemId;
	}

	private bool isInteracting = true;

	public void EndInteract()
	{
		isInteracting = false;
	}

	public void SuccessInteract(int playerId)
	{
		if (System.TryGiveItem(playerId, itemId, itemType) == false)
		{
			Debug.LogError($"{itemId}, {itemType} : 획득 실패");
		}
	}

	public bool TryStartInteract(int playerId)
	{
		isInteracting = true;
		return true;
	}

	public bool IsInteractable()
	{
		return isInteracting == false;
	}
}
