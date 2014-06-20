using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Svelto.Tasks
{
	public class SerialTaskCollection: TaskCollection
	{
		public event Action		onComplete;
		
		override public float 	progress { get { return _progress + _subProgress;} }
		 
		public SerialTaskCollection():base()
		{
			_progress = 0.0f;
			_subProgress = 0.0f;

			_token = null;
		}

		public SerialTaskCollection(object token):base()
		{
			_progress = 0.0f;
			_subProgress = 0.0f;

			_token = token;
		}
		
		override public IEnumerator GetEnumerator()
		{
			isRunning = true;
			
			int startingCount = registeredEnumerators.Count;
			
			while (registeredEnumerators.Count > 0) 
			{
				//create a new stack for each task
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				//let`s get the first available task
				IEnumerator task = registeredEnumerators.Dequeue();
				//put in the stack
				stack.Push(task);

				while (stack.Count > 0) 
				{
					IEnumerator ce = stack.Peek(); //get the current task to execute

					if (ce is AsyncTask)
						(ce as AsyncTask).token = _token;
	             
					if (ce.MoveNext() == false) 
					{
						_progress = (float)(startingCount - registeredEnumerators.Count) / (float)startingCount;
						_subProgress = 0;

						stack.Pop(); //task is done (the iteration is over)
					}
					else 
					{
						if (ce.Current != ce && ce.Current != null)  //the task returned a new IEnumerator (or IEnumerable)
						{	
							if (ce.Current is WWW || ce.Current is YieldInstruction)
								yield return ce.Current; //YieldInstructions are special cases and must be handled by Unity. They cannot be wrapped!
							else
							if (ce.Current is IEnumerable)	
								stack.Push(((IEnumerable)ce.Current).GetEnumerator()); //it's pushed because it can yield another IEnumerator on its own
							else 
							if (ce.Current is IEnumerator)	
								stack.Push(ce.Current as IEnumerator); //it's pushed because it can yield another IEnumerator on its own
						}
						
						if (ce is AsyncTask) //asyn
							_subProgress = (ce as AsyncTask).task.progress * (((float)(startingCount - (registeredEnumerators.Count)) / (float)startingCount) - progress);
						
						if (ce is EnumeratorWithProgress) //asyn
							_subProgress = (ce as EnumeratorWithProgress).progress * (((float)(startingCount - (registeredEnumerators.Count)) / (float)startingCount) - progress);
					}

                    yield return null; //the tasks are not done yet
				}
			}
			
			isRunning = false;
			
			if (onComplete != null)
				onComplete();
		}
		
		float 	_progress;
		float 	_subProgress;
		object	_token;
	}
}

