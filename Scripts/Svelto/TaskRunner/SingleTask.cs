using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace Svelto.Tasks
{
	public class SingleTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public SingleTask(IEnumerator enumerator)
		{
			SerialTasks task = new SerialTasks();
			
			task.Add(enumerator);
			
			_enumerator = task.GetEnumerator();
		}
		
		public bool MoveNext()
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

