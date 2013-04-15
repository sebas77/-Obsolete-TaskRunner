using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Svelto.Tasks
{
	public class SerialTasks: Tasks
	{
		public event Action			onComplete;
		 
		public SerialTasks():base()
		{}
		
		override public IEnumerator GetEnumerator()
		{
			isRunning = true;
			
			Debug.Log("Serialized Tasks Started, number of tasks: " + registeredEnumerators.Count);
			
			while (registeredEnumerators.Count > 0) 
			{
				//create a new stack for each task
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				//let`s get the first available task
				IEnumerator task = registeredEnumerators[0];
				//put in the stack
				stack.Push (task);
				
				while (stack.Count > 0) 
				{
					IEnumerator ce = stack.Peek(); //get the current task to execute
	             
					if (ce.MoveNext() == false) 
						stack.Pop(); //task is done (the iteration is over)
					else 
					if (ce.Current != ce && ce.Current != null)  //the task returned a new IEnumerator (or IEnumerable)
					{	
						if (ce.Current is IEnumerable)	
							stack.Push(((IEnumerable)ce.Current).GetEnumerator()); //it's pushed because it can yield another IEnumerator on its own
						else 
						if (ce.Current is IEnumerator)	
							stack.Push(ce.Current as IEnumerator); //it's pushed because it can yield another IEnumerator on its own
						else
						if (ce.Current is WWW || ce.Current is YieldInstruction) //we assume that these cannot ever yield an IEnumerator
							yield return ce.Current;
					}
					
					if (ce is ParallelTasks) //a set of parallel tasks is due to start
						Debug.Log ("New Set of Parallel Tasks Started from SerialTasks");
	 				
					yield return null; //the tasks are not done yet
				} 
				
				registeredEnumerators.RemoveAt(0); //task done, move to the next one if any
			}
			
			isRunning = false;
			
			Debug.Log ("All Serialized Tasks Ended");
			
			if (onComplete != null)
				onComplete();
		}
	}
}

