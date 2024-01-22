using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 하나씩 붙어있는 모듈이다.
/// 시스템에 가까운 기능을 하지만, 씬 모듈 시스템에 의해 관리되는 컴포넌트라는 개념
/// </summary>

public abstract class SceneModule : MonoBehaviour
{
    [SerializeField] private PrefabLinkedUISystem uiSystem = null;

	[SerializeField] protected SceneType mySceneType = SceneType.Title;
	protected float sceneOpenDeltaTime = 0.0f;

	protected virtual void Reset()
	{
        switch(this)
		{
			case TitleSceneModule:
				mySceneType = SceneType.Title;
                break;
            case MatchSceneModule:
                mySceneType = SceneType.Match;
                break;
            case BattleSceneModule:
                mySceneType = SceneType.Battle;
                break;
        }
    }

    public Transform RequestSystemGameObject(MonoSystem system)
    {
        var type = system.GetType();
        var typeName = type.Name;

        var g = new GameObject(typeName);
        var tr = g.transform;

        transform.SetPositionAndRotation(default, default);

        tr.SetParent(transform);
        tr.SetPositionAndRotation(default, default);
        tr.SetAsLastSibling();

        return tr;
    }

    public virtual void OnEnter(SceneModuleParam param)
	{
		sceneOpenDeltaTime = 0.0f;

        uiSystem.OnEnter(param);

	}

    public async virtual UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await UniTask.Yield();
    }

	public virtual void OnExit()
	{
		uiSystem.OnExit();
	}

	public async virtual UniTask OnPrepareExitRoutine()
	{
        await UniTask.Yield();
    }

    public virtual void OnFixedUpdate(int deltaFrameCount, float deltaTime)
    {
        
    }

    public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		sceneOpenDeltaTime += deltaTime;
    }

	public virtual void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

	public virtual void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

    protected virtual void OnGUI()
    {

    }

    protected virtual void OnDrawGizmos()
    {

    }

    public virtual void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}
}
