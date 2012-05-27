using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Tasks;

public class TaskRunner: MonoBehaviour
{
	static GameObject _instance;
		
	public void Run (IEnumerable task)
	{
		Run (task.GetEnumerator ());
	}
	
	public void Run (IEnumerator task)
	{
		if (this.enabled == true)
			StartCoroutine (task);
	}
	
	public void RunSync (IEnumerable task)
	{
		RunSync (task.GetEnumerator ());
	}
	
	public void RunSync (IEnumerator task)
	{
		while (task.MoveNext() == true)
			;
	}
	
	void Awake ()
	{
		if (_instance == null)
			_instance = gameObject;
		else 
			DestroyImmediate (this); //only one task runner for each project please
	}

	void OnDestroy ()
	{
		_instance = null; 
        
		StopAllCoroutines ();
	}
}
	
public delegate void TasksComplete ();


 