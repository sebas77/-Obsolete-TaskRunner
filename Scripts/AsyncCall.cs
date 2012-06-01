using System;
using System.Collections;
using UnityEngine;
using Tasks;

public delegate void AsyncCallComplete (System.Object obj);

public class AsyncCall
{
	private event AsyncCallComplete	onComplete = null;

	private System.Object 			_obj;
	
	public void RunOnComplete (AsyncCallComplete func)
	{
		onComplete = func;
		
		TaskRunner.Instance.Run (yieldable (_obj));
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


