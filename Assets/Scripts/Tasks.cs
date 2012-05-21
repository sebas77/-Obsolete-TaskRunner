using System;
using System.Collections;
using System.Collections.Generic;

namespace Tasks
{
	abstract public class Tasks: IEnumerable
	{
		protected List<IEnumerator> 	registeredEnumerators { get; private set; }
		public bool						isRunning { protected set; get; }
		
		public Tasks()
        {
			registeredEnumerators = new List<IEnumerator>();
		}
		
		public void Add(ITask task)
        {
			if (task == null)
				throw new ArgumentNullException();
			
           Add(new EnumerableTask(task));
        }
		
		public void Add(IEnumerable enumerable)
        {
			if (enumerable == null)
				throw new ArgumentNullException();
			
           registeredEnumerators.Add(enumerable.GetEnumerator());
        }
 
        public void Add(IEnumerator enumerator)
        {
			if (enumerator == null)
				throw new ArgumentNullException();
			
			registeredEnumerators.Add(enumerator);
        }
		
		abstract public IEnumerator GetEnumerator();
	}
}

