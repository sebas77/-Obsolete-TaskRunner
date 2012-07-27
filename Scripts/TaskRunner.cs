using System.Collections;
using UnityEngine;

public class TaskRunner
{
	private MonoBehaviour _runner;
	
	static private TaskRunner _instance;
		
	static public TaskRunner Instance
	{
		get 
		{
			InitInstance ();
			
			return _instance;
		}
	}
		
	public void Run(IEnumerable task)
	{
		Run(task.GetEnumerator());
	}
	
	public void Run(IEnumerator task)
	{
		if (_runner != null && _runner.enabled == true)
		{
			_runner.gameObject.active = true;
			_runner.StartCoroutine(task);
		}
	}
	
	public void RunSync(IEnumerable task)
	{
		RunSync(task.GetEnumerator());
	}
	
	public void RunSync(IEnumerator task)
	{
		while (task.MoveNext() == true);
	}
	
	public void Stop()
	{
		if (_runner != null)
			_runner.StopAllCoroutines();
	}
	
	static void InitInstance ()
	{
		if (_instance == null)
		{
			GameObject go = new GameObject("TaskRunner");
			
			_instance = new TaskRunner();
			_instance._runner = go.AddComponent<MonoBehaviour>();
			
			GameObject.DontDestroyOnLoad(go);
		}
	}
}
	



 