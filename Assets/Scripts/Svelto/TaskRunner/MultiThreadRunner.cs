using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace Svelto.Tasks
{
	public class MultiThreadRunner: IRunner
    {
		public MultiThreadRunner()
		{
			paused = false;	
			stopped = false;
			_priority = ThreadPriority.Normal;
		}

		public MultiThreadRunner(ThreadPriority priority)
		{
			paused = false;	
			stopped = false;
			_priority = priority;
		}

		public void StartCoroutine(IEnumerator task)
        {	
			stopped = false;
			paused = false;

			PausableTask stask;

			if (task is PausableTask)
				stask = task as PausableTask;
			else
				stask = new PausableTask(task, this); //ptask uses a single task internally

			Thread oThread = new Thread(new ThreadStart(() => { while (stask.MoveNext() == true); }));
			
			oThread.IsBackground = true;
			oThread.Priority = _priority;
			oThread.Start();
        }

		public void StartCoroutine(IEnumerable task)
        {
			StartCoroutine(task.GetEnumerator());
        }

        public void StopAllCoroutines()
        {
			stopped = true;
        }
		
		public bool paused { set; get; }
		public bool stopped { private set; get; }

		ThreadPriority _priority;
    }
}
