using UnityEngine;
using UnityEngine.EventSystems;

public class Singleton
{
	protected SingletonBehaviour behaviour = null;
	protected static bool isReleased = false;

	protected Singleton()
	{

	}

	public void Initialize(SingletonBehaviour behaviour)
    {
		this.behaviour = behaviour;
        behaviour.RegisterSingleton(this);

        OnAwakeInstance();
	}

	public void Release()
	{
		OnRelease();

		if (behaviour)
			behaviour.UnRegisterSingleton(this);
        
		isReleased = true;
	}

	protected virtual void OnAwakeInstance()
	{

	}

	protected virtual void OnRelease()
	{

	}

	public virtual void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}

// 아직 그냥 싱글톤임
// ScriptableObject로 바꿀 수 있을 지 고민 필요함
public abstract class Singleton<T> : Singleton where T : Singleton, new()
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (isReleased)
			{
				Debug.LogError($"{typeof(T)} 타입의 이미 해제된 싱글톤에 접근했습니다.");
				return null;
			}

			if (instance == null)
			{
				instance = new T();
				instance.Initialize(SingletonBehaviour.Instance);
			}

			return instance;
		}
	}
}
