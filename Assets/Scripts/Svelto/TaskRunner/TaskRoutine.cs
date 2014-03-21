using System;

namespace Svelto.Tasks
{
	public class TaskRoutine
	{
		internal TaskRoutine(PausableTask task)
		{
			_task = task;
		}

		public void Start()
		{
			_task.Start();
		}
		
		public void Stop()
		{
			_task.Stop();
		}
		
		string 			_name;
		PausableTask 	_task;
	}
}

