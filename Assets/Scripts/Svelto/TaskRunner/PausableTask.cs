using System.Collections;

namespace Svelto.Tasks
{
	internal class PausableTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public PausableTask(IEnumerator enumerator, IRunner runner)
		{
			_enumerator = new SingleTask(enumerator); //SingleTask is needed to be able to pause sub tasks
			
			_runner = runner;
		}
		
		public bool MoveNext()
		{
			if (_stopped)
				return false;
			
			if (_runner.paused == false)
				return _enumerator.MoveNext();
	
			return true;
		}
		
		public void Reset()
		{
			_enumerator.Reset();
		}

		public void Start()
		{
			_runner.StartCoroutine(this);
		}
		
		public void Stop()
		{
			_stopped = true;
		}
		
		IRunner 		_runner;
		bool			_stopped = false;
		IEnumerator 	_enumerator;
	}
}

