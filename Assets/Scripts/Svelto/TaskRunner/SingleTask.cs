using System.Collections;

/// <summary>
/// Single Task
///
/// - This is a more powerful version of passing a single IEnumerator as parameter of TaskRunner.Run
/// - The main difference is that a single task can execute other tasks returned by the single task
/// </summary>

namespace Svelto.Tasks
{
	public class SingleTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public SingleTask(IEnumerator enumerator)
		{
			SerialTasks task = new SerialTasks();
			
			task.Add(enumerator);
			
			_enumerator = task.GetEnumerator();
		}
		
		public SingleTask(IEnumerator enumerator, System.Action onComplete)
		{
			SerialTasks task = new SerialTasks();
			
			task.Add(enumerator);
			
			task.onComplete += onComplete;
			
			_enumerator = task.GetEnumerator();
		}
		
		virtual public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}
		public void Reset()
		{
			_enumerator.Reset();
		}
		
		private IEnumerator 		_enumerator;
	}
}

