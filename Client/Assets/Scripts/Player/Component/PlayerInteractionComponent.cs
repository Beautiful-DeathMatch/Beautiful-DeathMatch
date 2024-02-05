using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
	[SerializeField] private ThirdPersonController controller;
	[SerializeField] private Transform cameraTransform = null;

	private LayerMask interactionLayerMask;
	private float interactionMaxDistance = 8.0f;

	private IInteractable currentInteractableObject = null;

	public event Action<IInteractable> onHoldInteract = null;
	public event Action<IInteractable> onSuccessInteract = null;

	public event Action onCancelInteract = null;

	public float currentInteractionTime { get; private set; } = 0.0f;
	public float objectMaxInteractionTime { get; private set; } = 0.01f; // 상호 작용 오브젝트 쪽으로 뺄 수 있다면 -> 뺐음

	private int playerId = -1;

	public void SetPlayerId(int playerId)
	{
		this.playerId = playerId;
	}
	
	private void Awake()
	{
		interactionLayerMask = ~LayerMask.GetMask("Player"); // 인터렉션 레이 무시용 레이어 마스크
		cameraTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		controller.onHoldInteract += OnHoldInteract;
		controller.onCancelInteract += OnCancelInteract;
	}

	private void OnDisable()
	{
		controller.onHoldInteract -= OnHoldInteract;
		controller.onCancelInteract -= OnCancelInteract;

		if (currentInteractableObject != null)
		{
			currentInteractableObject.EndInteract();
		}
	}

	private void Update()
	{
		Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionMaxDistance, Color.blue, 0.3f);

		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionMaxDistance, interactionLayerMask)) // Ray 충돌 감지 시
		{
			currentInteractableObject = hit.transform.GetComponent<IInteractable>();
		}
		else
		{
			currentInteractableObject = null;
        }
	}

	private void OnHoldInteract()
	{
		if (currentInteractableObject == null || currentInteractableObject.IsInteractable(playerId) == false) 
			return;
		// 상호작용 도중 상호작용 물체가 바뀔 경우 새 Start 되도록 예외 필요할듯 -> 이때 Cancel 로직이 동작하지 않을 듯 하므로 새 로직 필요

		if (currentInteractionTime == 0.0f)
		{
			currentInteractableObject.TryStartInteract(playerId);
		}

        currentInteractionTime += Time.deltaTime;
        onHoldInteract?.Invoke(currentInteractableObject);

		objectMaxInteractionTime = currentInteractableObject.maxInteractionTime;
		if (currentInteractionTime >= objectMaxInteractionTime)
		{
			OnSuccessInteract();
		}
	}

	private void OnCancelInteract() // F키를 떼야만 동작함, F 누르고 있는 상태에서 인터랙션 물체를 벗어나도 동작해야 함 <= 수정 필요
	{
		currentInteractionTime = 0.0f;

		if (currentInteractableObject != null)
		{
			currentInteractableObject.EndInteract();
			currentInteractableObject = null;
        }

		onCancelInteract?.Invoke();
	}

	private void OnSuccessInteract()
	{
		currentInteractionTime = 0.0f;

		if (currentInteractableObject != null)
		{
			currentInteractableObject.SuccessInteract(playerId);
            onSuccessInteract?.Invoke(currentInteractableObject);

            currentInteractableObject = null;
		}
	}

	public IInteractable GetCurrentInteractableObject()
	{
		return currentInteractableObject;
	}

}
