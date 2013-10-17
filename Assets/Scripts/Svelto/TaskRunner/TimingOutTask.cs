using System.Collections;
using System;

namespace Svelto.Tasks
{
	public class TimingOutTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public TimingOutTask(float milliseconds, System.Action action)
		{
			_enumerator = Timeout(action, DateTime.Now.AddMilliseconds(milliseconds));
		}
		
		IEnumerator Timeout(System.Action action, DateTime endTime)
		{
			while (DateTime.Now < endTime)
				yield return null;
			
			action();
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

