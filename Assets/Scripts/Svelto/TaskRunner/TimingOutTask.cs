using System;
using System.Collections;

namespace Svelto.Tasks
{
	public class TimingOutTask: IEnumerable
	{
        public TimingOutTask(float milliseconds, Action action):this(milliseconds, action, false)
		{}
		
		public TimingOutTask(float milliseconds, Action action, bool repeat)
		{
			if (repeat == true)
				action();

            _action = action;
            _milliseconds = milliseconds;
            _repeat = repeat;
		}
		
		IEnumerator Timeout(Action action, float milliseconds, bool repeat)
		{
            DateTime endTime = DateTime.UtcNow.AddMilliseconds(milliseconds);
				
			while (DateTime.UtcNow < endTime)
				yield return null;
			
			action();
			
			if (repeat == true)
				yield return Timeout(action, milliseconds, true);
		}

        public IEnumerator GetEnumerator()
        {
            return Timeout(_action, _milliseconds, _repeat);
        }

        Action  _action;
        float   _milliseconds;
        bool    _repeat;
	}
}

