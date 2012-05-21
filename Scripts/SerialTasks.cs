using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Tasks
{
	public class SerialTasks: Tasks
    {
		public event TasksComplete			onComplete;
		 
        public SerialTasks():base()
        {}
		
		override public IEnumerator GetEnumerator()
		{
			isRunning = true;
			
			while (registeredEnumerators.Count > 0)
			{
				//create a new stack for each task
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				//let`s get the first available task
				IEnumerator task = registeredEnumerators[0];
				//put in the stack
				stack.Push(task);
				
				while (stack.Count > 0)
            	{
	                IEnumerator ce = stack.Peek(); //get the current task to execute
	             
					if (ce.MoveNext() == false)
					{
						stack.Pop(); //task is done (the iteration is over)
						
						Debug.Log("Serialized Task Done");
					}
	                else
					if (ce.Current != ce && ce.Current != null) 
					{	//the task returned a new IEnumerator (or IEnumerable)
						//that will be pushed in the stack to be executed next iteration
						Debug.Log(ce.Current.GetType());
						//the task could yield another IEnumerator
						if (ce.Current is IEnumerable)	
							stack.Push(((IEnumerable)ce.Current).GetEnumerator());
						else
						if (ce.Current is IEnumerator)	
							stack.Push(ce.Current as IEnumerator);
					}
					
					if (ce is ParallelTasks)
					{//a set of parallel tasks is due to start
						Debug.Log("New Set of Parallel Tasks Started from SerialTasks");
					}
	 				
					yield return null; //the tasks are not done yet
            	} 
								
				registeredEnumerators.RemoveAt(0); //task done, move to the next one if any
			}
			
			isRunning = false;
			
			Debug.Log("All Serialized Tasks Ended");
			
			if (onComplete != null)
				onComplete();
        }
	}
}

