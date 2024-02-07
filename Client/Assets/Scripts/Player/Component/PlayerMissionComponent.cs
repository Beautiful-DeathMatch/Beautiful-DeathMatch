using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미션 컴포넌트와 상호 작용을 통해 시스템에 클리어를 알리는 컴포넌트
/// </summary>
public class PlayerMissionComponent : MonoComponent<MissionSystem>
{
	[SerializeField] private PlayerInteractionComponent interactionComponent = null;

	/// <summary>
	/// 이것저것 컴포넌트에 이벤트를 등록하여, 적당한 때에 TryCompleteMission을 호출하도록 한다.
	/// </summary>

	private int playerId = -1;

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}

	private void OnEnable()
	{
		interactionComponent.onSuccessInteract += OnSuccessInteract;
	}

	private void OnDisable()
	{
		interactionComponent.onSuccessInteract -= OnSuccessInteract;
	}


	private void OnSuccessInteract(IInteractable interactableObject)
	{
		/*
		if (interactableObject == null)
			return;

		if (interactableObject is MonoBehaviour obj)
		{
			var missionObj = obj.GetComponent<MissionComponent>();
			if (missionObj == null)
				return;

			missionObj.TryCompleteMission(ENUM_MISSION_TYPE.Interaction, playerId);
		}
		*/
	}
	
}
