using UnityEngine;
using System.Collections;
using Svelto.Tasks;

//Multithread support is totally experiment
//It's not used in produced, so I don't know 
//if it works properly
public class ExampleParallelTasksManagedMultiThread : MonoBehaviour 
{
	int i;
    MultiThreadRunner _runner = new MultiThreadRunner();
	
	bool variableThatCouldHaveBeenUseful;
	MultiThreadParallelTaskCollection pt; //try to change the number of concurrent threads allowed. Leave it empty to leat the thread pool find the best number
    SerialTaskCollection st = new SerialTaskCollection();
	// Use this for initialization
	void Start ()
    {
        pt = new MultiThreadParallelTaskCollection(_runner);
        pt.onComplete += ReDoIt; //hardcore

        ReDoIt();
    }

    void OnDestroy() 
    {
        _runner.StopAllCoroutines();
    }

    private void ReDoIt()
    {
        //the order of st is supposed to be granted
        //this must happen when everything is done

        st.Add(Print("s1"));
        st.Add(Print("s2"));
        st.Add(DoSomethingAsynchonously());
        st.Add(Print("s3"));
        st.Add(Print("s4"));

        //the order of pt as they come

        pt.Add(Print("p1"));
        pt.Add(Print("p2"));
        pt.Add(Print("p3"));
        pt.Add(DoSomethingAsynchonously2());
        pt.Add(Print("p4"));
        pt.Add(st);
        pt.Add(Print("p5"));
        pt.Add(Print("p6"));
        pt.Add(Print("p7"));

        _runner.StartCoroutine(pt.GetEnumerator());
    }

    void Update()
	{
		if (Input.anyKeyDown)
		{
			if (_paused == false)
			{
				Debug.LogWarning("Paused!");
                _runner.paused = true; 
				_paused = true;
			}
			else
			{
				Debug.LogWarning("Resumed!");
				_paused = false;
                _runner.paused = false;
			}
		}
	}
	
	IEnumerator DoSomethingAsynchonously()
	{
        Debug.Log("after s2, this one, wait 3 seconds");

        for (i = 0; i < 10000000; i++)
	        yield return i;
			
		Debug.Log("index " + i);
	}

    IEnumerator DoSomethingAsynchonously2()
	{
        Debug.Log("thread started, wait 5 seconds");

        for (i = 0; i < 13000000; i++)
	        yield return i;
			
		Debug.Log("index " + i);
	}
	
	IEnumerator Print(string i)
	{
		Debug.Log(i);
		yield return null;
	}
	
	private bool _paused = false;
}
