using System.Collections;

namespace Svelto.Tasks
{
	public class AsyncTask: IEnumerable
	{
		public ITask task { get; private set; }
			
		public AsyncTask(ITask task)
		{
			this.task = task;
		}
		
		/// <summary>
		/// Gets the enumerator and execute the task
		/// The task is meant to be executed once. It can be
		/// both synchronous or asynchronous.
		/// If synchronous it must set isDone to true
		/// if asynchronous it must set isDone once the async 
		/// call is done 
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			task.Execute(); 
			
			while (task.isDone == false)
				yield return task;
		}
		
		public override string ToString()
		{
			return task.ToString();
		}
	}
}

