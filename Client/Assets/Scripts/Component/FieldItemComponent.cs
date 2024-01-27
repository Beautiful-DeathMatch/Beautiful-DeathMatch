using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum ENUM_INTERACT_TYPE
{
	NORMAL = 0,
	ITEM = 1,
}


public interface IInteractable
{
	ENUM_INTERACT_TYPE Type { get; }

	bool IsInteractable(int playerId);
	bool TryStartInteract(int playerId);
	void SuccessInteract(int playerId);
    void EndInteract();
}

public class FieldItemComponent : MonoComponent<ItemSystem>, IInteractable
{
	[SerializeField] private ENUM_ITEM_TYPE itemType = ENUM_ITEM_TYPE.None;
	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.ITEM;

	private int itemId = -1;
	private int currentInteractingPlayerId = -1;

	public void SetItemId(int itemId)
	{
		this.itemId = itemId;
	}

	private bool isInteracting = true;

	public void EndInteract()
	{
		isInteracting = false;

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
            Debug.Log($"{playerId}와 {itemId} : {itemType}가 상호 작용에 성공... 필드 아이템이 제거됩니다.");

            // 나중에 System으로 돌려준다.
            Destroy(gameObject);
        }
    }

    public bool TryStartInteract(int playerId)
	{
		Debug.Log($"{playerId}와 {itemId} : {itemType}가 상호 작용 시작...");

		currentInteractingPlayerId = playerId;

        isInteracting = true;
		return true;
	}

	public bool IsInteractable(int playerId)
	{
		return isInteracting == false || isInteracting && currentInteractingPlayerId == playerId;
	}
}
