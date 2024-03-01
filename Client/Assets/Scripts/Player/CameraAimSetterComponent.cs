using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraAimSetterComponent : MonoComponent<PrefabLinkedUISystem>
{
    [SerializeField] private Transform cameraOrigin = null;
	[SerializeField] private Transform cameraTarget = null;

	[SerializeField] private float aimDistance;

	private LayerMask aimLayerMask;

	private void Awake()
	{
		aimLayerMask = LayerMask.GetMask("Player");
	}

	private void Update()
	{
		UpdateTargetPosition(cameraOrigin.position, cameraOrigin.forward, aimDistance, aimLayerMask);
	}

	private void OnPreRender() 
	{
		UpdateAimPosition();
	}

	private void UpdateTargetPosition(Vector3 origin, Vector3 dir, float distance, LayerMask mask)
	{
		if (Physics.Raycast(origin, dir, out var hit, distance, mask))
		{
			cameraTarget.position = hit.transform.position;
		}
		else
		{
			cameraTarget.position = origin + dir * aimDistance;
		}
		Debug.DrawRay(origin, dir * distance, Color.green, 0.3f);
	}

	private void UpdateAimPosition()
	{
		var mainWindow = System.GetMainWindow<BattleMainWindow>();
		if (mainWindow == null)
			return;

		var mainWindowRect = mainWindow.GetComponent<RectTransform>();
		var screenPos = GetTargetScreenPos(mainWindowRect);
		
		mainWindow.UpdateAimPosition(screenPos);
	}

	public Vector2 GetTargetScreenPos(RectTransform canvasRect)
	{
		Vector2 viewPointPos = Camera.main.WorldToViewportPoint(cameraTarget.position);

		float x = viewPointPos.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f;
		float y = viewPointPos.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f;

		return new Vector2(x, y);
	}
}
