using Svelto.Tasks.Internal;
using System.Collections;
using System.Threading;

namespace Svelto.Tasks
{
	public class MultiThreadRunner: IRunner
    {
		public MultiThreadRunner()
		{
			paused = false;	
			stopped = false;
		}

		public MultiThreadRunner(ThreadPriority priority)
		{
			paused = false;	
			stopped = false;
		}

        public void StartCoroutine(IEnumerator task)
        {	
			stopped = false;
			paused = false;

            ThreadPool.QueueUserWorkItem((stateInfo) => 
            { 
                while (_stopped == false)
                {
                    if (task.MoveNext() == false)
                        break;

                    lock (_locker)
                        while (_paused == true)
                            Monitor.Wait(_locker);
                }
            });
        }

        public void StopAllCoroutines()
        {
             stopped = true;
        }

		public bool paused 
        { 
            set
            {
                lock (_locker)                 // Let's now wake up the thread by
                {
                    if (_paused == true && value == false)
                    {
                        _paused = value;

                        Monitor.PulseAll(_locker);
                    }
                    else
                        _paused = value;
                }
            }
            get { return _paused; } 
        }

		public bool stopped { private set { _stopped = value; Thread.MemoryBarrier(); } get { return _stopped; } }

        volatile bool _stopped;
        volatile bool _paused;

        static readonly object _locker = new object();
    }
}
