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
				InitInstance ();
			
			return _instance;
		}
	}
		
	public void Run(IEnumerable task)
	{
		Run(task.GetEnumerator());
	}
	
	public void Run(IEnumerator task)
	{
		_runner.StartCoroutine(new SingleTask(task));
	}
	
	public void RunSync(IEnumerable task)
	{
		RunSync(task.GetEnumerator());
	}
	
	public void RunSync(IEnumerator task)
	{
		IEnumerator taskToRun = new SingleTask(task);
		
		while (taskToRun.MoveNext() == true);
	}
	
	public TaskRoutine RunManaged(IEnumerable task)
	{
		return RunManaged(task.GetEnumerator());
	}
	
	public TaskRoutine RunManaged(IEnumerator task)
	{
		PausableTask ptask = new PausableTask(task, _runner); // a pausable task IS a single task
		
		Run(ptask);
		
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
	
	public void Destroy()
	{
		Stop();
		
		_runner.Destroy();
				
		_instance = null;
	}
	
	static void InitInstance ()
	{
		_instance = new TaskRunner();
		_instance._runner = new MonoTask();
	}
}
	



 