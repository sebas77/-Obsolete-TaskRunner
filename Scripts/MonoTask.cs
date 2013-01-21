using System.Collections;
using UnityEngine;
using Tasks;

class MonoTask:MonoBehaviour
{
	IEnumerator _enumerator;
	
	public bool paused { set; private get; }
	
	void Awake()
	{
		paused = false;	
	}

	public void StartCoroutineManaged(IEnumerator task)
	{
		SingleTask tasks = new SingleTask(task);
		
		_enumerator = tasks.GetEnumerator();
	}

	void FixedUpdate()
	{
		if (paused == false)
		{
			if (_enumerator != null && _enumerator.MoveNext() == false)
			{
				_enumerator = null;
			}
		}
	}
}