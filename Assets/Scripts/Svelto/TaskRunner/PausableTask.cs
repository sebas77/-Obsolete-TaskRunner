using System.Collections;

namespace Svelto.Tasks
{
	public class PausableTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public PausableTask(IEnumerator enumerator, IRunner runner)
		{
			_enumerator = new SingleTask(enumerator); //SingleTask is needed to be able to pause sub tasks
			
			_runner = runner;
		}

		public PausableTask(SingleTask enumerator, IRunner runner)
		{
			_enumerator = enumerator;

			_runner = runner;
		}
		
		public bool MoveNext()
		{
			if (_stopped || _runner.stopped)
				return false;
			
			if (_runner.paused == false && _paused == false)
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

		public void Pause()
		{
			_paused = true;
		}

		public void Resume()
		{
			_paused = false;
		}
		
		public void Stop()
		{
			_stopped = true;
			_enumerator = null;
		}
		
		IRunner 		_runner;
		bool			_stopped = false;
		IEnumerator 	_enumerator;
		bool			_paused = false;
	}
}

