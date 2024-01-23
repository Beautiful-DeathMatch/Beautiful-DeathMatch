using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_INTERACT_TYPE
{
	NORMAL = 0,
	ITEM = 1,
}


public interface IInteractableObject
{
	ENUM_INTERACT_TYPE Type { get; }

	bool IsInteractable();
	bool TryInteract(int uniqueId);
    void EndInteract();
}

public class FieldItemComponent : MonoComponent<ItemSystem>, IInteractableObject
{
	public ENUM_ITEM_TYPE ItemType { get; private set; }
	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.ITEM;

	private bool isInteractable = true;

	public void SetItemType(ENUM_ITEM_TYPE itemType)
	{
		this.ItemType = itemType;
	}

	public virtual void EndInteract()
	{
		Debug.Log($"상호 작용 종료");
		isInteractable = true;
	}

	public virtual bool TryInteract(int uniqueId)
	{
		Debug.Log($"{uniqueId}와의 상호 작용");
		isInteractable = false;
		return true;
	}

	public bool IsInteractable()
	{
		return isInteractable;
	}
}
