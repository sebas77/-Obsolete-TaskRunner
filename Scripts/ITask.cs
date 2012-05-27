using System;
using System.Collections;

namespace Tasks
{
	public interface ITask
	{
		event 		TasksComplete	onComplete;
		
		bool		isDone { get; }
		
		void		Execute ();	
	}
}

