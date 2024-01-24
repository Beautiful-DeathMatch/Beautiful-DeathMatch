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
	[SerializeField] private ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.ITEM;

	private bool isInteracting = true;

	public virtual void EndInteract()
	{
		isInteracting = false;
	}

	public virtual bool TryInteract(int uniqueId)
	{
		isInteracting = true;
		return true;
	}

	public bool IsInteractable()
	{
		return isInteracting == false;
	}
}
