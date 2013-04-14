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
	 * To check if a task isDone, the isDone
	 * getter must be used.
	 **/
	public interface ITask
	{
		event 		Action	onComplete;
		
		bool		isDone { get; }
		
		void		Execute ();	
	}
}

