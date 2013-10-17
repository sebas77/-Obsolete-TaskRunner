using System;
using System.Collections;

namespace Svelto.Tasks
{
	/*
	 * ITask is the interface of a generic Task
	 * a Task is not meant to be enumerated, it
	 * must be executed only once!
	 * A task can (and should) wrap an 
	 * asynchronous call though, so it can
	 * last over the time.
	 * once the task is completed, the isDone
	 * value must be set to true.
	 **/
	public interface ITask
	{
		event 		Action	onComplete;
		
		bool		isDone { get; }
		float		progress { get; }
		
		void		Execute ();	
	}
}

