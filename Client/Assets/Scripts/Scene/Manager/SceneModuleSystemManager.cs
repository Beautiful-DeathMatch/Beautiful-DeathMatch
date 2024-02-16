using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Start = -1,
    Title = 0,
    Match = 1,
    Battle = 2,
}

public class SceneModuleParam
{

}

public class SceneModuleSystemManager : Singleton<SceneModuleSystemManager>
{
    public event Action<SceneType> onSceneChanged = null;

    public SceneType CurrentSceneType => currentSceneType;
    private SceneType currentSceneType;

    private SceneModule currentSceneModule = null;
    private SceneModuleParam currentParam = null;

    private CancellationTokenSource cancellationSource = new CancellationTokenSource();
    private bool isLoadComplete = false;

    private Dictionary<string, Transform> systemTransformDictionary = new Dictionary<string, Transform>();

    protected override void OnAwakeInstance()
    {
        behaviour.onFixedUpdate += FixedUpdate;
        behaviour.onUpdate += Update;
        behaviour.onLateUpdate += LateUpdate;
        behaviour.onExit += OnExit;

		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnRelease()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        behaviour.onFixedUpdate -= FixedUpdate;
        behaviour.onUpdate -= Update;
        behaviour.onLateUpdate -= LateUpdate;
		behaviour.onExit -= OnExit;

		if (cancellationSource != null)
			cancellationSource.Cancel();
	}

    private void FixedUpdate()
    {
        if (currentSceneModule != null)
            currentSceneModule.OnFixedUpdate(Time.frameCount, Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (isLoadComplete == false)
            return;

        if (currentSceneModule != null)
            currentSceneModule.OnPrevUpdate(Time.frameCount, Time.deltaTime);

        if (currentSceneModule != null)
            currentSceneModule.OnUpdate(Time.frameCount, Time.deltaTime);

        if (currentSceneModule != null)
            currentSceneModule.OnPostUpdate(Time.frameCount, Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (isLoadComplete == false)
            return;

        if (currentSceneModule != null)
            currentSceneModule.OnLateUpdate(Time.frameCount, Time.deltaTime);
    }

    private void OnExit()
    {
		if (currentSceneModule != null)
			currentSceneModule.OnExit();
	}

    public void TryEnterSceneModule(SceneType sceneType, SceneModuleParam param = null)
    {
        currentParam = param;
        StartLoadSceneAsync(sceneType).ToCancellationToken(cancellationSource.Token);
    }

	public void SetSystemChild(MonoSystem system, MonoBehaviour childObj)
	{
		var type = system.GetType();
		var typeName = type.Name;

		if (systemTransformDictionary.TryGetValue(typeName, out var singletonTransform) == false)
		{
			singletonTransform = currentSceneModule.RequestSystemGameObject(system);
		}

		childObj.transform.SetParent(singletonTransform);
		childObj.transform.SetPositionAndRotation(default, default);
		childObj.transform.SetAsLastSibling();
	}


	private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMoad)
    {
        if (Enum.TryParse(scene.name, out currentSceneType))
        {
            onSceneChanged?.Invoke(currentSceneType);
            FinishLoadSceneAsync().ToCancellationToken(cancellationSource.Token);
        }

        systemTransformDictionary.Clear();
    }

    private async UniTask StartLoadSceneAsync(SceneType sceneType)
    {
        if(currentSceneModule)
        {
            await currentSceneModule.OnPrepareExitRoutine();
            currentSceneModule?.OnExit();
        }

        isLoadComplete = false;
        await LoadSceneAsync(sceneType, null);
    }

    private async UniTask LoadSceneAsync(SceneType sceneType, Action<float> OnSceneLoading)
    {
        var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        await operation.ToUniTask(Progress.Create((float progress) =>
        {
            if (progress >= 0.9f && operation.allowSceneActivation == false)
            {
				operation.allowSceneActivation = true;
				OnSceneLoading?.Invoke(1.0f);
			}
			else
            {
				OnSceneLoading?.Invoke(progress);
			}
		}));
    }

    private async UniTask FinishLoadSceneAsync()
    {
        currentSceneModule = UnityEngine.Object.FindObjectOfType<SceneModule>();
        if (currentSceneModule == null)
            return;

        await currentSceneModule.OnPrepareEnterRoutine(currentParam);
        currentSceneModule.OnEnter(currentParam);

        currentParam = null;
        isLoadComplete = true;
    }
}
