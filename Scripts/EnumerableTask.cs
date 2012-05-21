using System;
using System.Collections;

namespace Tasks
{
	public class EnumerableTask: IEnumerable
	{
		public ITask task {get; private set;}
			
		public EnumerableTask (ITask task)
		{
			this.task = task;
		}
		
		public IEnumerator GetEnumerator()
		{
			task.Execute();
			
			while (task.isDone == false)
				yield return null;
		}
		
		public override string ToString() 
		{
			return task.ToString();
		}
	}
}

