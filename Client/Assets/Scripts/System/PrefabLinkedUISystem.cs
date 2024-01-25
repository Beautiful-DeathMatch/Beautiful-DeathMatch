using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrefabLinkedUISystem : MonoSystem
{
	[SerializeField] private ResourceSystem resourceSystem = null;

    private EventSystem eventSystem;
	private Dictionary<Type, UIPopup> popupDictionary = new Dictionary<Type, UIPopup>();
	private Stack<UIPopup> popupStack = new Stack<UIPopup>();

	private UIMainWindow currentMainWindow = null;

#if UNITY_EDITOR
	protected override void OnReset()
    {
        base.OnReset();

		resourceSystem = FindObjectOfType<ResourceSystem>();
    }
#endif

	public override void OnEnter(SceneModuleParam sceneModuleParam)
	{
		SetMainWindow(SceneModuleSystemManager.Instance.CurrentSceneType, sceneModuleParam);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		base.OnUpdate(deltaFrameCount, deltaTime);

		currentMainWindow.OnUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnExit()
	{
		if (currentMainWindow != null)
		{
			currentMainWindow.OnExit();
		}
	}

	private void SetMainWindow(SceneType type, SceneModuleParam sceneModuleParam)
	{
		currentMainWindow = UnityEngine.Object.FindObjectOfType<UIMainWindow>();
		if(currentMainWindow != null)
		{
			currentMainWindow.SetOrder(0);
			currentMainWindow.OnEnter(sceneModuleParam);
		}
		else
		{
			Debug.LogError($"{type} 씬에 Main Window가 존재하지 않습니다.");
		}

		eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
		if (eventSystem == null)
		{
			Debug.LogError($"{type} 씬에 Event System이 존재하지 않습니다.");
		}
	}

	public TWindow GetMainWindow<TWindow>() where TWindow : UIMainWindow
	{
		return currentMainWindow as TWindow;
	}

	private void ReleasePopups(SceneType type)
	{
		popupStack.Clear();

		foreach (var popup in popupDictionary.Values)
		{
			Destroy(popup.gameObject);
		}

		popupDictionary.Clear();
	}

	public T OpenPopup<T>(UIParam param = null) where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup))
		{
			return OpenPopupInternal<T>(popup, param);
		}

        popup = CreatePopup<T>();
		return OpenPopupInternal<T>(popup, param);
    }

	private T CreatePopup<T>() where T : UIPopup
	{
		return resourceSystem.Instantiate<T>();
	}

	private T OpenPopupInternal<T>(UIPopup popup, UIParam param) where T : UIPopup
	{
		if (popup.TryOpen(param))
		{
			this.SetChildObject(popup);

			popup.SetOrder(popupStack.Count + 1);
			popupStack.Push(popup);
		}

		popupDictionary[typeof(T)] = popup;
		return popup as T;
	}

	public void ClosePopup<T>() where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup) == false)
			return;

		if (popup.TryClose() == false)
			return;

		Stack<UIPopup> tempStack = new Stack<UIPopup>();

		while(popupStack.Count > 0)
		{
			var peekPopup = popupStack.Pop();
			if (peekPopup == null)
				continue;

			if (peekPopup.GetType() == typeof(T))
				break;

			tempStack.Push(peekPopup);
		}

		while(tempStack.Count > 0)
		{
			popupStack.Push(tempStack.Pop());
		}
	}

	public void ClosePopup(Type type)
	{
		if (popupDictionary.TryGetValue(type, out var popup) == false)
			return;

		if (popup.TryClose() == false)
			return;

		Stack<UIPopup> tempStack = new Stack<UIPopup>();

		while (popupStack.Count > 0)
		{
			var peekPopup = popupStack.Pop();
			if (peekPopup == null)
				continue;

			if (peekPopup.GetType() == type)
				break;

			tempStack.Push(peekPopup);
		}

		while (tempStack.Count > 0)
		{
			popupStack.Push(tempStack.Pop());
		}
	}

	public T GetPopup<T>(bool includeDeactive = false) where T : UIPopup
	{
        if (popupDictionary.TryGetValue(typeof(T), out var popup))
        {
            var _popup = popup as T;
            if (_popup != null)
			{
				if (_popup.IsActive || includeDeactive)
				{
					return _popup;
                }
			}
        }

		return null;
    }
}
