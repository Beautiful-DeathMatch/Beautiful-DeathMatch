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

	public event Action<IInteractable> onPressInteract = null;
	public event Action offPressInteract = null;

	private bool isInteracting = false;

	private void Awake()
	{
		interactionLayerMask = ~LayerMask.GetMask("Player"); // 인터렉션 레이 무시용 레이어 마스크
		cameraTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		controller.IsInteracting += IsInteracting;

		controller.onPressInteract += OnPressInteract;
		controller.offPressInteract += OffPressInteract;
	}

	private void OnDisable()
	{
		controller.IsInteracting -= IsInteracting;

		controller.onPressInteract -= OnPressInteract;
		controller.offPressInteract -= OffPressInteract;
	}

	private void Update()
	{
		Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionMaxDistance, Color.blue, 0.3f);

		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionMaxDistance, interactionLayerMask)) // 충돌 감지 시
		{
			currentInteractableObject = hit.transform.GetComponent<IInteractable>();
		}

		if (currentInteractableObject == null)
		{
			isInteracting = false;
		}
	}

	private void OnPressInteract()
	{
		if (currentInteractableObject == null || currentInteractableObject.IsInteractable() == false)
			return;

		isInteracting = true;
		onPressInteract?.Invoke(currentInteractableObject);
	}

	private void OffPressInteract()
	{
		isInteracting = false;
		offPressInteract?.Invoke();
	}

	private bool IsInteracting()
	{
		return isInteracting;
	}
	
}
