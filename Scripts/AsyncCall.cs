using System;
using System.Collections;
using UnityEngine;
using Tasks;

public delegate void AsyncCallComplete(System.Object obj);

public class AsyncCall
{
	public event AsyncCallComplete	onComplete;
	
	static private TaskRunner _taskRunner = LookUp();
	
	static private TaskRunner LookUp()
    {
         TaskRunner taskRunner = (TaskRunner)GameObject.FindObjectOfType(typeof(TaskRunner));
		
		if (taskRunner == null)
			throw new ArgumentNullException("AsyncCall");
			
		return taskRunner;
    }
		
	public AsyncCall (System.Object obj)
	{
		onComplete = null;
		
		_taskRunner.Run(yieldable(obj));
	}
	
	private IEnumerator yieldable(System.Object obj)
	{
		yield return obj;
		
		if (onComplete != null)
			onComplete(obj);
	}
}


