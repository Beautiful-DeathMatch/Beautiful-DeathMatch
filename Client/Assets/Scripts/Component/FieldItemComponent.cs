using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public enum ENUM_INTERACT_TYPE
{
	NORMAL = 0,
	ITEM = 1,
	BOX = 2,
	MISSION = 3,
	CALL = 4,
	HELICOPTER = 5,
	ETC = 6
}


public interface IInteractable
{
	ENUM_INTERACT_TYPE Type { get; }
	float maxInteractionTime { get; }

	bool IsInteractable(int playerId);
	bool TryStartInteract(int playerId);
	void SuccessInteract(int playerId);
    void EndInteract();
}

public class FieldItemComponent : MonoComponent<ItemSystem>, IInteractable
{
	[SerializeField] private ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.ITEM;
	public float maxInteractionTime => 0.01f;

	private int itemId = -1;
	private int currentInteractingPlayerId = -1;

	public void SetItemId(int itemId)
	{
		this.itemId = itemId;
	}

	public void SetItemType(ENUM_ITEM_TYPE itemType)
	{
		if(itemType == ENUM_ITEM_TYPE.None)
			this.itemType = itemType;
	}

	public void EndInteract()
	{
        Debug.Log($"{currentInteractingPlayerId}와 {itemId} : {itemType}가 상호 작용 중도 종료");

		currentInteractingPlayerId = -1;
	}

	public void SuccessInteract(int playerId)
	{
		if (currentInteractingPlayerId != playerId)
			return;

		if (System.TryGiveItem(playerId, itemId, itemType) == false)
		{
			Debug.LogError($"플레이어 {playerId}가 {itemId}, {itemType} 획득에 실패");
		}
		else
        {
            Debug.Log($"{playerId}와 {itemId} : {itemType}가 상호 작용에 성공하였습니다.");
			System.TryDestroyFieldItem(itemId);
        }

		EndInteract();
	}

    public bool TryStartInteract(int playerId)
	{
		if (currentInteractingPlayerId != -1)
			return false;

		Debug.Log($"{playerId}와 {itemId} : {itemType}가 상호 작용 시작...");

		currentInteractingPlayerId = playerId;
		return true;
	}

	public bool IsInteractable(int playerId)
	{
		return (currentInteractingPlayerId == -1 || currentInteractingPlayerId == playerId) && !this.IsDestroyed();
	}

	public int GetItemId()
	{
		return itemId;
	}
}
