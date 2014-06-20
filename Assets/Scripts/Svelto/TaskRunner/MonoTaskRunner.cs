using UnityEngine;
using System.Collections;

namespace Svelto.Tasks
{
	class MonoTaskRunner: IRunner
	{
		MonoTaskBehaviour 	_component;
				
		public bool paused { set; get; }
		public bool stopped { private set; get; }
		
		void Init()
		{
			GameObject go = new GameObject("TaskRunner");

			go.hideFlags = HideFlags.HideInHierarchy;

			if ((_component = go.GetComponent<MonoTaskBehaviour>()) == null)
				_component = go.AddComponent<MonoTaskBehaviour>();

			paused = false;
			stopped = false;
		}
		
		public void StartCoroutine(IEnumerable task)
		{
			StartCoroutine(task.GetEnumerator());
		}
		
		public void StartCoroutine(IEnumerator task)
		{
			stopped = false;
			paused = false;

            if (_component == null)
                if (MonoTaskBehaviour.isQuitting == false)
					Init();
                else
                    return;

			PausableTask stask;

			if (task is PausableTask)
				stask = task as PausableTask;
			else
				stask = new PausableTask(task, this); //ptask uses a single task internally

			_component.gameObject.SetActive(true);
			_component.enabled = true;
			_component.StartCoroutine(stask);
		}

		public void StopAllCoroutines()
		{
			stopped = true;
		}
	}
}
