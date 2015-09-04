using System;
using System.Collections;

namespace Svelto.Tasks.Internal
{
	internal class PausableTask: IEnumerator
	{
		public object Current 		
        { 
            get 
            { 
                if (_enumerator != null)
                    return _enumerator.Current;

                return null;
            } 
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
            throw new NotImplementedException("Pausable Tasks do not support Reset");
        }
				 
		internal PausableTask(IEnumerator enumerator, IRunner runner)
		{
            if (enumerator is SingleTask || enumerator is PausableTask || enumerator is AsyncTask)
				throw new ArgumentException("Use of incompatible Enumerator, cannot be SingleTask/PausableTask/AsyncTask");

            _enumerator = enumerator;
			
			_runner = runner;
		}

        internal PausableTask(IRunner runner)
		{
            _runner = runner;
		}

		internal void Start(bool isSimple)
		{
            SetTask(_enumerator, isSimple);

            _runner.StartCoroutine(this);
		}

        internal void Start(IEnumerator task, bool isSimple)
		{
            SetTask(task, isSimple);

            _runner.StartCoroutine(this);
		}

		internal void Pause()
		{
			_paused = true;
		}

		internal void Resume()
		{
			_paused = false;
		}
		
		internal void Stop()
		{
			_stopped = true;
			_enumerator = null;
		}

        void SetTask(IEnumerator task, bool isSimple)
        {
            if (isSimple == false)
            {
                if (_enumerator is SingleTask)
                    (_enumerator as SingleTask).Reuse(task);
                else
                    _enumerator = new SingleTask(task);
            }
            else
                _enumerator = task;
        }
		
		IRunner 		_runner;
		bool			_stopped = false;
		IEnumerator 	_enumerator;
		bool			_paused = false;
	}
}

