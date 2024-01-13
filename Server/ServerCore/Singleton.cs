using System;

namespace ServerCore
{
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

	public class Singleton : IDisposable
	{
		private bool disposedValue;

		protected Singleton()
		{
			OnAwakeInstance();
        }

		// Dispose를 반드시 부르도록 하는 게 좋을까?
		~Singleton()
		{
			Dispose(disposing: false);
		}

		protected virtual void OnAwakeInstance()
		{

		}

		protected virtual void OnDestroyInstance()
		{

		}

		protected virtual void OnDisposeInstance()
		{

		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// 관리형 리소스
					OnDestroyInstance();
				}

				// 비관리형 리소스
				OnDisposeInstance();
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
