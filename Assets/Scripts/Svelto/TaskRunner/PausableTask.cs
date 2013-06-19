using System.Collections;

namespace Svelto.Tasks
{
	class PausableTask: SingleTask
	{
		public PausableTask(IEnumerator enumerator, MonoTask runner):base(enumerator)
		{
			_runner = runner;
		}
		
		public PausableTask(IEnumerator enumerator, System.Action onComplete, MonoTask runner):base(enumerator, onComplete)
		{
			_runner = runner;
		}
		
		override public bool MoveNext()
		{
			if (_runner.paused == false)
				return base.MoveNext();
			
			return true;
		}
		
		MonoTask _runner;
	}
}

