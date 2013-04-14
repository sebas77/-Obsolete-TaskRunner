using UnityEngine;
using System.Collections;
using Svelto.Tasks;

public class Example : MonoBehaviour 
{
	int i;
	
	bool variableThatCouldHaveBeenUseful;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(DoSomethingAsynchonously());
		
		TaskRunner.Instance.Run(new SingleTask(DoSomethingAsynchonously()));
	}
	
	IEnumerator DoSomethingAsynchonously()
	{
		variableThatCouldHaveBeenUseful = false;
		
	    yield return SomethingAsyncHappens();
		
		variableThatCouldHaveBeenUseful = true;
	
	    Debug.Log("index is: " + i);
	}
	
	IEnumerator SomethingAsyncHappens()
	{
	    for (i = 0; i < 100; i++)
	        yield return i;
	}
}
