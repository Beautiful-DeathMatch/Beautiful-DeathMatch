using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
	public class JobQueue
	{
		Queue<Action> jobQueue = new Queue<Action>();
		object lockObj = new object();

		bool isFlushed = false;

		public void Push(Action job)
		{
			bool isFlushed = false;

			lock (lockObj)
			{
				jobQueue.Enqueue(job);

				if (!this.isFlushed)
				{
					this.isFlushed = isFlushed = true;
				}
			}

			if (isFlushed)
			{
				Flush();
			}
		}

		private void Flush()
		{
			while (true)
			{
				var action = Pop();
				if (action == null)
					break;

				action.Invoke();
			}
		}

		private Action Pop()
		{
			lock (lockObj)
			{
				if (jobQueue.Count == 0)
				{
					isFlushed = false;
					return null;
				}

				return jobQueue.Dequeue();
			}
		}
	}
}
