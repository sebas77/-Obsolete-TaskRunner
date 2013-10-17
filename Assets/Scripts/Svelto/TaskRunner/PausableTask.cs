using System.Collections;

namespace Svelto.Tasks
{
	internal class PausableTask: SingleTask
	{
		public PausableTask(IEnumerator enumerator, IRunner runner):base(enumerator)
		{
			_runner = runner;
		}
		
		public PausableTask(IEnumerator enumerator, System.Action onComplete, MonoTask runner):base(enumerator, onComplete)
		{
			_runner = runner;
		}
		
		override public bool MoveNext()
		{
			if (_stopped)
				return false;
			
			if (_runner.paused == false)
				return base.MoveNext();
	
			return true;
		}
		
		public void Stop()
		{
			_stopped = true;
		}
		
		IRunner _runner;
		bool	_stopped = false;
	}
}

