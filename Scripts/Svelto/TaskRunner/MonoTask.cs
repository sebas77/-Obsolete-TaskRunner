using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Svelto.Tasks;

class MonoTask : MonoBehaviour
{
	List<IEnumerator> _enumerators;
	
	public bool paused { set; private get; }
	
	void Awake()
	{
		_enumerators = new List<IEnumerator>();
		
		paused = false;	
	}

	public void StartCoroutineManaged(IEnumerator task)
	{
		_enumerators.Add(new SingleTask(task).GetEnumerator());
	}

	void FixedUpdate()
	{
		if (paused == false)
			_enumerators.RemoveAll(enumerator => enumerator.MoveNext() == false);
	}
}