using UnityEngine;
using System.Collections;
using Svelto.Tasks;

public class ExampleParallelTasksManaged : MonoBehaviour 
{
	int i;
	
	bool variableThatCouldHaveBeenUseful;
	
	// Use this for initialization
	void Start () 
	{
		ParallelTaskCollection pt = new ParallelTaskCollection();
		SerialTaskCollection	st = new SerialTaskCollection();
		
		st.Add(Print("s1"));
		st.Add(Print("s2"));
		st.Add(DoSomethingAsynchonously());
		st.Add(Print("s3"));
		st.Add(Print("s4"));
		
		pt.Add(Print("1"));
		pt.Add(Print("2"));
		pt.Add(WWWTest("www.google.com"));
		pt.Add(WWWTest("http://download.thinkbroadband.com/5MB.zip"));
		pt.Add(WWWTest("www.ebay.com"));
		pt.Add(Print("3"));
		pt.Add(Print("4"));
		pt.Add(st);
		pt.Add(Print("5"));
		pt.Add(Print("6"));
		pt.Add(Print("7"));
			
		TaskRunner.Instance.Run(pt.GetEnumerator());
	}
	
	void Update()
	{
		if (Input.anyKeyDown)
		{
			if (_paused == false)
			{
				Debug.LogWarning("Paused!");
				TaskRunner.Instance.PauseManaged();
				_paused = true;
			}
			else
			{
				Debug.LogWarning("Resumed!");
				_paused = false;
				TaskRunner.Instance.ResumeManaged();
			}
		}
	}
	
	IEnumerator DoSomethingAsynchonously() //this can be awfully slow, since it is synched with the framerate
	{
		 for (i = 0; i < 500; i++)
	        yield return i;
			
		Debug.Log("index " + i);
	}
	
	IEnumerator Print(string i)
	{
		Debug.Log(i);
		yield return null;
	}
	
	IEnumerator WWWTest(string url)
	{
		WWW www = new WWW(url);
		
		yield return www;
		
		Debug.Log("www done:" + www.text);
	}
	
	private bool _paused = false;
}
