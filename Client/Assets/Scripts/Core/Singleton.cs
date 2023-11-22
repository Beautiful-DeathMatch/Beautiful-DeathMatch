
public class Singleton<T> : Singleton where T : Singleton, new()
{
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}

			return instance;
		}
	}

	private static T instance = null;
}

public class Singleton
{
	protected Singleton()
	{

	}
}