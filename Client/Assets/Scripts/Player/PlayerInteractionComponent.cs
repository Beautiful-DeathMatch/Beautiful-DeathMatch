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

	private IInteractableObject currentInteractableObject = null;

	public event Action<IInteractableObject> onPressInteract = null;
	public event Action offPressInteract = null;

	private bool isInteracting = false;

	private void Awake()
	{
		interactionLayerMask = ~LayerMask.GetMask("Player"); // 인터렉션 레이 무시용 레이어 마스크
		cameraTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		controller.isInteracting += IsInteracting;

		controller.onPressInteract += OnPressInteract;
		controller.offPressInteract += OffPressInteract;
	}

	private void OnDisable()
	{
		controller.isInteracting -= IsInteracting;

		controller.onPressInteract -= OnPressInteract;
		controller.offPressInteract -= OffPressInteract;
	}

	private void Update()
	{
		Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionMaxDistance, Color.blue, 0.3f);

		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionMaxDistance, interactionLayerMask)) // 충돌 감지 시
		{
			currentInteractableObject = hit.transform.GetComponent<IInteractableObject>();
		}

		if (currentInteractableObject == null)
		{
			isInteracting = false;
		}
	}

	private void OnPressInteract()
	{
		if (currentInteractableObject == null || currentInteractableObject.IsInteractable() == false)
		{
			isInteracting = false;
			return;
		}

		isInteracting = true;
		onPressInteract?.Invoke(currentInteractableObject);
	}

	private void OffPressInteract()
	{
		isInteracting = false;
	}

	private bool IsInteracting()
	{
		return isInteracting;
	}
	
}
