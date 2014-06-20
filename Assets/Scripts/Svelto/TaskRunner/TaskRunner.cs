using System.Collections;
using Svelto.Tasks;

public class TaskRunner
{
	private IRunner _runner;
	
	static private TaskRunner _instance;
		
	static public TaskRunner Instance
	{
		get 
		{
			if (_instance == null)
				InitInstance();
			
			return _instance;
		}
	}
		
	public void Run(IEnumerable task)
	{
		if (task == null)
			return;

		Run(task.GetEnumerator());
	}
	
	public void Run(IEnumerator task)
	{
		if (task == null)
			return;

		_runner.StartCoroutine(task);
	}
	
	public void RunSync(IEnumerable task)
	{
		if (task == null)
			return;

		RunSync(task.GetEnumerator());
	}
	
	public void RunSync(IEnumerator task)
	{
		if (task == null)
			return;

		IEnumerator taskToRun = new SingleTask(task);
		
		while (taskToRun.MoveNext() == true);
	}
	
	public void RunManaged(IEnumerable task)
	{
		if (task == null)
			return;

		RunManaged(task.GetEnumerator());
	}

	public void RunManaged(IEnumerator task)
	{
		ResumeManaged();

		if (task == null)
			return;

		_runner.StartCoroutine(task); 
	}

	public TaskRoutine CreateTask(IEnumerable task)
	{
		if (task == null)
			return null;

		return CreateTask(task.GetEnumerator());
	}

	public TaskRoutine CreateTask(IEnumerator task)
	{
		if (task == null)
			return null;

		PausableTask ptask = new PausableTask(task, _runner);
		
		return new TaskRoutine(ptask);
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
		_instance._runner 	= new MonoTaskRunner();
	}
}
	



 