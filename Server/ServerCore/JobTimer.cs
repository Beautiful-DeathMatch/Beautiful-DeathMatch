using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace ServerCore
{
	struct JobTimerElement : IComparable<JobTimerElement>
	{
		public int execTick; // 실행 시간
		public Action execFunction;

		public JobTimerElement(int execTick, Action action)
		{
			this.execTick = execTick;
			this.execFunction = action;
		}

		public void Invoke()
		{
			execFunction?.Invoke();
		}

		public int CompareTo(JobTimerElement other)
		{
			return other.execTick - execTick;
		}
	}

	public class JobTimer
	{
		PriorityQueue<JobTimerElement> jobElementQueue = new PriorityQueue<JobTimerElement>();
		object lockObj = new object();

		public void Start(Action action)
		{
			Push(action, 0);
		}

		public void Push(Action action, int tickDeltaTime)
		{
			JobTimerElement job = new JobTimerElement(Environment.TickCount + tickDeltaTime, action);

			lock (lockObj)
			{
				jobElementQueue.Push(job);
			}
		}

		public void Tick()
		{
			while (true)
			{
				int now = Environment.TickCount;

				JobTimerElement job;

				lock (lockObj)
				{
					if (jobElementQueue.Count == 0)
						break;

					job = jobElementQueue.Peek();
					if (job.execTick > now)
						break;

					jobElementQueue.Pop();
				}

				job.Invoke();
			}
		}
	}
}
