using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class UIWindow : MonoBehaviour
{
	protected PrefabLinkedUISystem uiSystem = null;

    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler scaler;

#if UNITY_EDITOR
	private void Reset()
	{
        canvas = GetComponent<Canvas>();
		scaler = GetComponent<CanvasScaler>();

		uiSystem = FindObjectOfType<PrefabLinkedUISystem>();

		SetCanvas();
		SetScaler();
		SetOrder(0);
	}
#endif

	private void Awake()
	{
		uiSystem = FindObjectOfType<PrefabLinkedUISystem>();
	}

	private void SetCanvas()
	{
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
	}

	private void SetScaler()
	{
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		scaler.referencePixelsPerUnit = 100;
	}

	public void SetOrder(int orderIndex)
	{
		canvas.sortingOrder = LayerHelper.GetSortingLayer(gameObject, orderIndex);
	}
}
