using Svelto.Tasks.Internal;
using System;
using System.Collections;

namespace Svelto.Tasks
{
	public class TaskRoutine
	{
        internal TaskRoutine(PausableTask task)
		{
            _task = task;
		}

        public TaskRoutine SetEnumeratorProvider(Func<IEnumerator> taskGenerator)
		{
            _taskGenerator = taskGenerator;

            return this;
		}

        public TaskRoutine Start(bool isSimple = true)
		{
            if (_taskGenerator == null)
                throw new Exception("A enumerator provider is required to enable this function, please use SetEnumeratorProvider before to call start");

            _task.Start(_taskGenerator(), isSimple);

            return this;
		}

        public TaskRoutine Start(IEnumerator taskGenerator, bool isSimple = true)
		{
            _task.Start(taskGenerator, isSimple);

            return this;
		}

		public void Stop()
		{
			_task.Stop();
		}

		public void Pause()
		{
			_task.Pause();
		}
		
		public void Resume()
		{
			_task.Resume();
		}

		PausableTask 	  _task;
        Func<IEnumerator> _taskGenerator;
    }
}

