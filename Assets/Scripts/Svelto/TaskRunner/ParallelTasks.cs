using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WWWEnumerator:IEnumerator
{
	WWW _www;

	public WWWEnumerator(WWW www)
	{
		_www = www;
	}

	public object Current	{ get { return _www; }}
	
	public bool MoveNext ()
	{
		return _www.isDone == false;
	}
	
	public void Reset ()
	{
	}
}

namespace Svelto.Tasks
{
	public class ParallelTasks: Tasks
    {
        public event Action		onComplete;
		
		private List<Stack<IEnumerator>> listOfStacks;
 
        public ParallelTasks():base()
        {
			listOfStacks = new List<Stack<IEnumerator>>();
		}
		
		override public IEnumerator GetEnumerator()
		{
			isRunning = true;
			
			listOfStacks.Clear();
			
			foreach (IEnumerator enumerator in registeredEnumerators)
            {
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				
				stack.Push(enumerator);
				
				listOfStacks.Add(stack);
			}
			
			registeredEnumerators.Clear();
			
			Debug.Log("Parallel Tasks Started, number of tasks: " + listOfStacks.Count);
				
			while (listOfStacks.Count > 0)
			{
				for (int i = 0; i < listOfStacks.Count; ++i)
				{
					Stack<IEnumerator> stack = listOfStacks[i];
					
					if (stack.Count > 0)
		            {
		                IEnumerator ce = stack.Peek(); //without popping it.
						
		                if (ce.MoveNext() == false)
							stack.Pop(); //now it can be popped
		                else //ok the iteration is not over
						if (ce.Current != null && ce.Current != ce)
						{
							if (ce.Current is IEnumerable)	//what we got from the enumeration is an IEnumerable?
								stack.Push(((IEnumerable)ce.Current).GetEnumerator());
							else
							if (ce.Current is IEnumerator)	//what we got from the enumeration is an IEnumerator?
								stack.Push(ce.Current as IEnumerator);
							else
							if (ce.Current is WWW)
								stack.Push(new WWWEnumerator(ce.Current as WWW));
							else
							if (ce.Current is YieldInstruction)
								yield return ce.Current; //be careful, this cannot be executed in parallel. A yield instruction will pause all the other tasks!
						}
		            }
					else
					{
						listOfStacks.RemoveAt(i);
						i--;
					}
				}
				
				yield return null;
			}
			
			Debug.Log("All Parallel Tasks Ended");
			
			isRunning = false;
			
			if (onComplete != null)
				onComplete();
        }
    }
}