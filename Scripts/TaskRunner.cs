using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Tasks;

public class TaskRunner: MonoBehaviour
{
	static private TaskRunner _instance;
		
	static public TaskRunner Instance
	{
		get 
		{
			if (_instance == null)
			{
				TaskRunner instance = (MonoBehaviour.FindObjectOfType(typeof(TaskRunner)) as TaskRunner);

				if (instance)
					_instance = instance;
				else
				{
					GameObject go = new GameObject("TaskRunner");
					_instance = go.AddComponent<TaskRunner>();
				}
			}
			
			return _instance;
		}
	}
		
	public void Run(IEnumerable task)
	{
		Run (task.GetEnumerator ());
	}
	
	public void Run(IEnumerator task)
	{
		if (this.enabled == true)
			StartCoroutine (task);
	}
	
	public void RunSync(IEnumerable task)
	{
		RunSync (task.GetEnumerator ());
	}
	
	public void RunSync(IEnumerator task)
	{
		while (task.MoveNext() == true);
	}
	
	public void Stop()
	{
		StopAllCoroutines();
	}
	
	void Awake ()
	{
		if (_instance == null || _instance != this)
		{
			Destroy(_instance);
			_instance = this;
			return;
		}
	}

	void OnDestroy()	//clean up if the gameobject is destroyed
	{
		_instance = null;
        
		StopAllCoroutines();
	}
}
	
public delegate void TasksComplete();


 