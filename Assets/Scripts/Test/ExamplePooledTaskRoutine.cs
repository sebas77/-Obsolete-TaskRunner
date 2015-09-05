using UnityEngine;
using System.Collections;
using Svelto.Tasks;

public class ExamplePooledTaskRoutine : MonoBehaviour 
{
	int i;
	
	bool variableThatCouldHaveBeenUseful;
    private TaskRoutine _taskRountine;
    private bool _paused;

    ParallelTaskCollection pt = new ParallelTaskCollection();
	SerialTaskCollection	st = new SerialTaskCollection();

    // Use this for initialization
    void Start () 
	{
		Application.targetFrameRate = 20;
			
		_taskRountine = TaskRunner.Instance.CreateTask().SetEnumeratorProvider(ResetTask); //The Task routine is pooled! You could have used Start directly, but you need to use SetEnumeratorProvider if you want restart the TaskRoutine later
	}

    IEnumerator ResetTask() //this is the suggested why to reset complicated tasks
    {
        st.Reset();
        pt.Reset();

        st.Add(Print("s1"));
        st.Add(Print("s2"));
		st.Add(DoSomethingAsynchonously());
		st.Add(Print("s3"));
        st.Add(Print("s4"));
		
		pt.Add(Print("1"));
		pt.Add(Print("2"));
		pt.Add(Print("3"));
		pt.Add(Print("4"));
		pt.Add(Print("5"));
		pt.Add(st);
		pt.Add(Print("6"));
		pt.Add(WWWTest ());
		pt.Add(Print("7"));
		pt.Add(Print("8"));

        return pt.GetEnumerator();
    }

    IEnumerator Print(string i)
	{
		Debug.Log(i);
		yield return null;
	}

    IEnumerator WWWTest()
	{
		WWW www = new WWW("www.google.com");
		
		yield return www;
		
		Debug.Log("www done:" + www.text);
	}

    void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (_paused == false)
			{
				Debug.LogWarning("Paused!");

                _taskRountine.Pause();
				_paused = true;
			}
			else
			{
				Debug.LogWarning("Resumed!");

				_paused = false;
                _taskRountine.Resume();
			}
		}

        if (Input.GetKeyUp(KeyCode.S))
            _taskRountine.Start();
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
