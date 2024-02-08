using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class MissionComponent : MonoComponent<MissionSystem>, IInteractable
{
    public ENUM_INTERACT_TYPE Type => ENUM_INTERACT_TYPE.MISSION;
	public float maxInteractionTime => 0f;
	private int currentInteractingPlayerId = -1;

	[SerializeField] public ENUM_MISSION_TYPE missionType = ENUM_MISSION_TYPE.None;

	public void EndInteract()
	{
        Debug.Log($"{currentInteractingPlayerId}와 {missionType} : 상호 작용 중도 종료");

		currentInteractingPlayerId = -1;
	}

	public void SuccessInteract(int playerId)
	{
		if (currentInteractingPlayerId != playerId)
			return;
        
		if (System.TryMissionStart(playerId, missionType) == false)
		{
			Debug.LogError($"플레이어 {playerId}가 {missionType} 상호 작용에 실패");
		}
		else
        {
            Debug.Log($"{playerId}와 {missionType} : 상호 작용에 성공하였습니다.");
        }

		EndInteract();
	}

    public bool TryStartInteract(int playerId)
	{
		if (currentInteractingPlayerId != -1)
			return false;

		Debug.Log($"{playerId}와 {missionType} : 상호 작용 시작...");

		currentInteractingPlayerId = playerId;
		return true;
	}

	public bool IsInteractable(int playerId)
	{
		bool isMissionCompleted = System.IsMissionCompleted(playerId, missionType);
		return (currentInteractingPlayerId == -1 || currentInteractingPlayerId == playerId) && !this.IsDestroyed() && !isMissionCompleted;
	}

    public bool TryMissionComplete(ENUM_MISSION_TYPE missionType, int playerId)
    {
        if (this.missionType != missionType)
            return false;

        return true;
    }
}
