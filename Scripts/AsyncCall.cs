using System;
using System.Collections;
using UnityEngine;
using Tasks;

public delegate void AsyncCallComplete (System.Object obj);

public class AsyncCall
{
	private event AsyncCallComplete	onComplete = null;

	private System.Object 			_obj;
	
	static private TaskRunner 		_taskRunner = LookUp ();
	
	static private TaskRunner LookUp ()
	{
		TaskRunner taskRunner = (TaskRunner)GameObject.FindObjectOfType (typeof(TaskRunner));
		
		if (taskRunner == null)
			throw new ArgumentNullException ("AsyncCall");
			
		return taskRunner;
	}
	
	public AsyncCall OnComplete (AsyncCallComplete func)
	{
		onComplete = func;
		
		return this;
	}
	
	public void Run ()
	{
		_taskRunner.Run (yieldable (_obj));
	}
		
	public AsyncCall (System.Object obj)
	{
		onComplete = null;
		
		_obj = obj;
	}
	
	private IEnumerator yieldable (System.Object obj)
	{
		yield return obj;
		
		if (onComplete != null)
			onComplete (obj);
	}
}


