using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
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

	private bool isInteracting = false;

	private float currentInteractionTime = 0.0f;
	private const float MaxInteractionTime = 5.0f;

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
		controller.IsInteracting += IsInteracting;

		controller.onPressInteract += OnHoldInteract;
		controller.offPressInteract += OnCancelInteract;
	}

	private void OnDisable()
	{
		controller.IsInteracting -= IsInteracting;

		controller.onPressInteract -= OnHoldInteract;
		controller.offPressInteract -= OnCancelInteract;

		if (currentInteractableObject != null)
		{
			currentInteractableObject.EndInteract();
		}
	}

	private void Update()
	{
		Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionMaxDistance, Color.blue, 0.3f);

		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionMaxDistance, interactionLayerMask)) // 충돌 감지 시
		{
			currentInteractableObject = hit.transform.GetComponent<IInteractable>();
		}
		else
		{
			if (currentInteractableObject != null)
			{
				currentInteractableObject.EndInteract();
			}
		}

		if (currentInteractableObject == null)
		{
			isInteracting = false;
		}
	}

	private void OnHoldInteract()
	{
		if (currentInteractableObject == null || currentInteractableObject.IsInteractable() == false)
			return;

		isInteracting = true;
		currentInteractionTime += Time.deltaTime;

		if (currentInteractionTime == 0.0f)
		{
			currentInteractableObject.TryStartInteract(playerId);
		}

		onHoldInteract?.Invoke(currentInteractableObject);

		if (currentInteractionTime >= MaxInteractionTime)
		{
			OnSuccessInteract();
		}
	}

	private void OnCancelInteract()
	{
		isInteracting = false;
		currentInteractionTime = 0.0f;

		currentInteractableObject.EndInteract();
		onCancelInteract?.Invoke();
	}

	private void OnSuccessInteract()
	{
		isInteracting = false;
		currentInteractionTime = 0.0f;

		currentInteractableObject.SuccessInteract(playerId);
		currentInteractableObject.EndInteract();

		onSuccessInteract?.Invoke(currentInteractableObject);
	}

	private bool IsInteracting()
	{
		return isInteracting;
	}
	
}
