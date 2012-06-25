using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Tasks
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
			
			foreach (IEnumerator enumerator in registeredEnumerators)
            {
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				
				stack.Push(enumerator);
				
				listOfStacks.Add(stack);
			}
			
			registeredEnumerators.Clear();
				
			while (listOfStacks.Count > 0)
			{
				for (int i = 0; i < listOfStacks.Count; ++i)
				{
					Stack<IEnumerator> stack = listOfStacks[i];
					
					if (stack.Count > 0)
		            {
		                IEnumerator ce = stack.Peek(); //without popping it.
						
		                if (ce.MoveNext() == false)
						{
							Debug.Log("Parallel Task " + i + " Done");
		                    
							stack.Pop(); //now it can be popped
						}
		                else //ok the iteration is not over
						if (ce.Current != null && ce.Current != ce)
						{
							if (ce.Current is IEnumerable)	//what we got from the enumeration is an IEnumerable?
								stack.Push(((IEnumerable)ce.Current).GetEnumerator());
							else
							if (ce.Current is IEnumerator)	//what we got from the enumeration is an IEnumerator?
								stack.Push(ce.Current as IEnumerator);
						}
		 				
						yield return null;
		            }
					else
					{
						listOfStacks.RemoveAt(i);
						i--;
					}
				}
			}
			
			Debug.Log("Parallel Tasks Ended");
			
			isRunning = false;
			
			if (onComplete != null)
				onComplete();
        }
    }
}