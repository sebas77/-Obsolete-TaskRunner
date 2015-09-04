using System.Collections;
using Svelto.Tasks;
using Svelto.Tasks.Internal;
using System;

public class TaskRunner
{
	static TaskRunner _instance;
    
    static public TaskRunner Instance
	{
		get 
		{
			if (_instance == null)
				InitInstance();
			
			return _instance;
		}
	}
		
	public void Run(IEnumerator task, bool isSimple = true)
	{
		if (task == null)
			return;

         _taskRoutinePool.Start(task, isSimple);
	}

	public void RunSync(IEnumerator task)
	{
		if (task == null)
			return;

		IEnumerator taskToRun = new SingleTask(task);
		
		while (taskToRun.MoveNext() == true);
	}

    public TaskRoutine CreateTask()
	{
        return _taskRoutinePool.RetrieveTask();
	}

    public void PauseManaged()
	{
		_runner.paused = true;
	}
	
	public void ResumeManaged()
	{
		_runner.paused = false;
	}
	
	public void Stop()
	{
		if (_runner != null)
			_runner.StopAllCoroutines();
	}

	static void InitInstance()
	{
		_instance 			= new TaskRunner();
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
        _instance._runner = new MonoRunner();
#else
        _instance._runner = new MultiThreadRunner();
#endif
        _instance._taskRoutinePool = new TaskRoutinePool(_instance._runner);
	}

    TaskRoutinePool _taskRoutinePool;
    IRunner         _runner;
}
	



 
