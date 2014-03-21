using UnityEngine;
using System.Collections;

namespace Svelto.Tasks
{
	class MonoTaskRunner : IRunner
	{
		MonoTaskBehaviour _component;
		
		public bool paused { set; get; }
		
		public MonoTaskRunner()
		{
			GameObject go = GameObject.Find("TaskRunner");
				
			if (go == null)
			{
				go = new GameObject("TaskRunner");
					
				GameObject.DontDestroyOnLoad(go);

				go.hideFlags = HideFlags.HideInHierarchy;
			}
			
			if ((_component = go.GetComponent<MonoTaskBehaviour>()) == null)
				_component = go.AddComponent<MonoTaskBehaviour>();
			
			_component.onLevelWasLoaded = OnLevelWasLoaded;
			
			paused = false;	
		}
		
		void OnLevelWasLoaded () 
		{
			StopAllCoroutines();
		}
		
		public void StartCoroutine(IEnumerator task)
		{
			if (_component == null)
				return;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			_component.gameObject.SetActive(true);
#else
			_runner.gameObject.active = true;
#endif			
			_component.enabled = true;
				
			_component.StartCoroutine(task);
		}

		public void StopAllCoroutines()
		{
			_component.StopAllCoroutines();
		}
		
		public void Destroy()
		{
#if !UNITY_EDITOR
				GameObject.Destroy(_component.gameObject);
#else
				GameObject.DestroyImmediate(_component.gameObject);
#endif
		}
	}
}