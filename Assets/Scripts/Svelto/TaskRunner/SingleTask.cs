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
			if (enumerator is TaskCollection || enumerator is SingleTask)
				_enumerator = enumerator;
			else
			{
				_task = new SerialTaskCollection();
			
				_task.Add(enumerator);
			
				_enumerator = _task.GetEnumerator();
			}
			
			_onComplete = null;
		}
		
		public SingleTask(IEnumerator enumerator, System.Action onComplete):this(enumerator)
		{
			_onComplete = onComplete;
		}
		
		public bool MoveNext()
		{
			if (_enumerator.MoveNext() == false)
			{
				if (_onComplete != null)
					_onComplete();
				
				return false;
			}
			return true;
		}
		
		public void Reset()
		{
			_enumerator.Reset();
		}
		
		IEnumerator		_enumerator;
		SerialTaskCollection		_task;
		System.Action 	_onComplete;
	}
}

