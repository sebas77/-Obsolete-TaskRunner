#region Usings
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Tasks;

#endregion

namespace Test
{
	[TestFixture]
    public class TaskRunnerTests
	{
		TaskRunner _taskRunner;

        #region TaskImplementation

		class Task : ITask
		{
			public event TasksComplete onComplete;

			public bool isDone { get; private set; }

			public void Execute ()
			{
				isDone = false;

				//wait synchronously for 1 second
				//usually it is an async operation
				IEnumerator e = WaitHalfSecond ();
				while (e.MoveNext());

				isDone = true;

				if (onComplete != null)
					onComplete ();
			}

			private IEnumerator WaitHalfSecond ()
			{
				float time = Time.realtimeSinceStartup;

				while (Time.realtimeSinceStartup - time < 0.5)
					yield return null;
			}
		}

        #endregion

        #region EnumerableImplementation

		class Enumerable : IEnumerable
		{
			public IEnumerator GetEnumerator ()
			{
				float time = Time.realtimeSinceStartup;

				while (Time.realtimeSinceStartup - time < 1)
					yield return null;
			}
		}

        #endregion

        #region Setup/Teardown
		
		SerialTasks serialTasks1;
		SerialTasks serialTasks2;
		ParallelTasks parallelTasks1;
		ParallelTasks parallelTasks2;

		ITask task1;
		ITask task2;
		
		Enumerable iterable1 = new Enumerable ();
		Enumerable iterable2 = new Enumerable ();
		

		[SetUp]
		public void Setup ()
		{
			serialTasks1 = new SerialTasks ();
			parallelTasks1 = new ParallelTasks ();
			serialTasks2 = new SerialTasks ();
			parallelTasks2 = new ParallelTasks ();

			task1 = new Task ();
			task2 = new Task ();
			
			_taskRunner = (TaskRunner)GameObject.FindObjectOfType (typeof(TaskRunner));
		}

        #endregion
		
		[Test]
		public void TestSingleTaskExecution ()
		{
			float time = Time.realtimeSinceStartup;
			bool test1Done = false;

			task1.onComplete += () => {
				test1Done = true; };

			task1.Execute ();

			Assert.That (test1Done == true && task1.isDone == true && Time.realtimeSinceStartup - time >= 0.5);
		}
		
		void SetupAndRunSerialTasks ()
		{
			serialTasks1.Add (task1);
			serialTasks1.Add (task2);
			
			_taskRunner.RunSync (serialTasks1);
		}

		void SetupAndRunParallelTasks ()
		{
			parallelTasks1.Add (task1);
			parallelTasks1.Add (task2);
			
			_taskRunner.RunSync (parallelTasks1);
		}
		
		[Test]
		public void TestSerializedTasksAreExecutedInSerial ()
		{
			bool allDone = false;
			
			serialTasks1.onComplete += () => { allDone = true; };
			
			SetupAndRunSerialTasks ();

			Assert.That (allDone == true);
		}
		
		[Test]
		public void TestTask1IsExecutedBeforeTask2 ()
		{
			bool test1Done = false;
			
			task1.onComplete += () => {	test1Done = true; };
			task2.onComplete += () => { Assert.That (test1Done == true);};
			
			SetupAndRunSerialTasks ();
		}
		
		[Test]
		public void TestTask1AndTask2AreExecutedInParallel ()
		{
			bool allDone = false;

			parallelTasks1.onComplete += () => { allDone = true; };
			
			SetupAndRunParallelTasks();

			Assert.That (allDone, Is.EqualTo (true));
		}

		[Test]
		public void TestEnumerableAreExecutedInSerial ()
		{
			bool allDone = false;

			serialTasks1.onComplete += () => { allDone = true; };

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);
			
			_taskRunner.RunSync (serialTasks1);

			Assert.That (allDone == true);
		}

		[Test]
		public void TestEnumerableAreExecutedInParallel ()
		{
			bool allDone = false;

			parallelTasks1.onComplete += () => { allDone = true; };

			parallelTasks1.Add (iterable1);
			parallelTasks1.Add (iterable2);
			
			_taskRunner.RunSync (parallelTasks1);
			
			Assert.That (allDone == true);
		}
		
		[Test]
		public void TestParallelTasks1IsExecutedBeforeParallelTask2 ()
		{
			bool parallelTasks1Done = false;

			parallelTasks1.Add (task1);
			parallelTasks1.Add (iterable1);
			parallelTasks1.onComplete += () => { parallelTasks1Done = true; };

			parallelTasks2.Add (task2);
			parallelTasks2.Add (iterable2);
			parallelTasks2.onComplete += () => { Assert.That(parallelTasks1Done == true); };
			
			serialTasks1.Add (parallelTasks1);
			serialTasks1.Add (parallelTasks2);
			
			_taskRunner.RunSync (serialTasks1);
		}

		[Test]
		public void TestParallelTasksAreExecutedInSerial ()
		{
			bool allDone = false;
			bool parallelTasks1Done = false;
			bool parallelTasks2Done = false;

			parallelTasks1.Add (task1);
			parallelTasks1.Add (iterable1);
			parallelTasks1.onComplete += () => { parallelTasks1Done = true; };

			parallelTasks2.Add (task2);
			parallelTasks2.Add (iterable2);
			parallelTasks2.onComplete += () => { parallelTasks2Done = true; };
			
			serialTasks1.Add (parallelTasks1);
			serialTasks1.Add (parallelTasks2);
			serialTasks1.onComplete += () => { allDone = true; };

			_taskRunner.RunSync (serialTasks1);

			Assert.That (parallelTasks1Done == true, "parallelTasks1Done");
			Assert.That (parallelTasks2Done == true, "parallelTasks2Done");
			Assert.That (allDone == true, "allDone");
		}

		[Test]
		public void TestSerialTasksAreExecutedInParallel ()
		{
			bool allDone = false;
			bool serialTasks1Done = false;
			bool serialTasks2Done = false;

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);
			serialTasks1.onComplete += () => {serialTasks1Done = true; };

			serialTasks2.Add (task1);
			serialTasks2.Add (task2);
			serialTasks2.onComplete += () => {serialTasks2Done = true; };

			parallelTasks1.Add (serialTasks1);
			parallelTasks1.Add (serialTasks2);
			parallelTasks1.onComplete += () => {allDone = true; };

			_taskRunner.RunSync (parallelTasks1);

			Assert.That (serialTasks1Done == true);
			Assert.That (serialTasks2Done == true);
			Assert.That (allDone == true);
		}
	}
}
