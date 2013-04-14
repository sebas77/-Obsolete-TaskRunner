using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Svelto.Tasks
{
	public class SingleTask: IEnumerable
	{
		class WWWEnumerator:IEnumerator
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
		
		private IEnumerator 		_enumerator;
	
		public event Action			onComplete;
		 
		public SingleTask(IEnumerator enumerator):base()
		{
			_enumerator = enumerator;
		}
		
		public IEnumerator GetEnumerator()
		{
			if (_enumerator != null)
			{
				Stack<IEnumerator> stack = new Stack<IEnumerator>();
				
				IEnumerator task = _enumerator;
				
				stack.Push(task);
				
				while (stack.Count > 0) 
				{
					IEnumerator ce = stack.Peek(); //get the current task to execute
	             
					if (ce.MoveNext() == false) 
						stack.Pop(); //task is done (the iteration is over)
					else 
					if (ce.Current != ce && ce.Current != null) 
					{	
						if (ce.Current is IEnumerable)	
							stack.Push(((IEnumerable)ce.Current).GetEnumerator());
						else 
						if (ce.Current is IEnumerator)	
							stack.Push(ce.Current as IEnumerator);
						else
						if (ce.Current is WWW)
							stack.Push(new WWWEnumerator(ce.Current as WWW));
					}
					
					yield return null; //the tasks are not done yet
				} 
				
				_enumerator = null;
			}
			
			if (onComplete != null)
				onComplete();
		}
	}
}

