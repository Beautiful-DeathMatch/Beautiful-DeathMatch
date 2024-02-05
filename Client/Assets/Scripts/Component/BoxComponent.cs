using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoxComponent : MonoComponent<ItemSystem>, IInteractable
{

	public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.BOX;
	public float maxInteractionTime => 3f;

    private int boxId = -1;
	private int currentInteractingPlayerId = -1;

	public void SetBoxId(int boxId)
	{
		this.boxId = boxId;
	}

	public void EndInteract()
	{
        Debug.Log($"{currentInteractingPlayerId}와 {boxId} : 상호 작용 중도 종료");

		currentInteractingPlayerId = -1;
	}

	public void SuccessInteract(int playerId)
	{
		if (currentInteractingPlayerId != playerId)
			return;
        
		if (System.TryGetRandomItemInBox(playerId, boxId) == false)
		{
			Debug.LogError($"플레이어 {playerId}가 {boxId} 획득에 실패");
		}
		else
        {
            Debug.Log($"{playerId}와 {boxId} : 상호 작용에 성공하였습니다.");
			System.TryDestroyBox(boxId);
        }

		EndInteract();
	}

    public bool TryStartInteract(int playerId)
	{
		if (currentInteractingPlayerId != -1)
			return false;

		Debug.Log($"{playerId}와 {boxId} : 상호 작용 시작...");

		currentInteractingPlayerId = playerId;
		return true;
	}

	public bool IsInteractable(int playerId)
	{
		return (currentInteractingPlayerId == -1 || currentInteractingPlayerId == playerId) && !this.IsDestroyed();
	}

}
