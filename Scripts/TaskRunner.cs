using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Tasks;

public class TaskRunner: MonoBehaviour
{
	static GameObject _gameObject;
	
	static private TaskRunner _instance;
	static public TaskRunner Instance {get { return _instance; }}
		
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
		if (_gameObject == null)
		{
			_gameObject = gameObject;
			_instance = this;
		}
		else 
			DestroyImmediate(this); //only one task runner for each project please
	}

	void OnDestroy()	//clean up if the gameobject is destroyed
	{
		_gameObject = null; 
		_instance = null;
        
		StopAllCoroutines();
	}
}
	
public delegate void TasksComplete();


 