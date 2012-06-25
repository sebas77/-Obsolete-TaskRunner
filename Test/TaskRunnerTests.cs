#region Usings
using System.Collections;
using Tasks;
using NUnit.Framework;
using UnityEngine;
using System.Threading;
#endregion

namespace Test
{
	[TestFixture]
    public class TaskRunnerTests
	{
		[SetUp]
		public void Setup ()
		{
			serialTasks1 = new SerialTasks();
			parallelTasks1 = new ParallelTasks();
			serialTasks2 = new SerialTasks();
			parallelTasks2 = new ParallelTasks();

			task1 = new Task(15);
			task2 = new Task(5);
			
			iterable1 = new Enumerable(15);
			iterable2 = new Enumerable(5);
			
			_taskRunner = TaskRunner.Instance;
		}
		
		[Test]
		public void TestSingleEnumerationBlockExecution()
		{
			IEnumerator enumerable = iterable1.GetEnumerator();
			
			while (enumerable.MoveNext() == true);

			Assert.That(iterable1.AllRight(), Is.True);
		}
		
		[Test]
		public void TestEnumerableAreExecutedInSerial()
		{
			bool allDone = false;

			serialTasks1.onComplete += () => { allDone = true; };

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);
			
			_taskRunner.RunSync (serialTasks1);

			Assert.That (allDone == true);
			Assert.That (iterable1.AllRight() == true);
			Assert.That (iterable2.AllRight() == true);
		}

		[Test]
		public void TestEnumerableAreExecutedInParallel()
		{
			bool allDone = false;
			bool test2MustFinishBeforeTest1 = false;
			
			iterable1.onComplete += () => { 
				Assert.That (test2MustFinishBeforeTest1 == true);
			};
			iterable2.onComplete += () => {	
				test2MustFinishBeforeTest1 = true; 
			};

			parallelTasks1.onComplete += () => { allDone = true; };

			parallelTasks1.Add (iterable1);
			parallelTasks1.Add (iterable2);
			
			_taskRunner.RunSync (parallelTasks1);
			
			Assert.That (allDone == true);
		}
		
		[Test]
		public void TestSingleTaskExecution()
		{
			task1.Execute();
			
			while (task1.isDone == false);

			Assert.That(task1.isDone == true);
		}
		
		[Test]
		public void TestSingleTaskExecutionCallsOnComplete()
		{
			bool test1Done = false;

			task1.onComplete += () => {
				test1Done = true; };
			
			task1.Execute();
			
			while (task1.isDone == false);

			Assert.That(test1Done == true);
		}
		
		[Test]
		public void TestSerializedTasksAreExecutedInSerial()
		{
			bool allDone = false;
			
			serialTasks1.onComplete += () => { allDone = true; };
			
			SetupAndRunSerialTasks();

			Assert.That(allDone == true);
		}
		
		[Test]
		public void TestTask1IsExecutedBeforeTask2()
		{
			bool test1Done = false;
			
			task1.onComplete += () => {	test1Done = true; };
			task2.onComplete += () => { Assert.That (test1Done == true); };
			
			SetupAndRunSerialTasks();
		}
		
		[Test]
		public void TestEnumerable1AndTask2AreExecutedInParallel()
		{
			bool allDone = false;
						
			parallelTasks1.onComplete += () => { allDone = true; };
				
			SetupAndRunParallelTasks();

			Assert.That(allDone, Is.EqualTo(true));
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
		
        #region TaskImplementation

		class Task : ITask
		{
			public event System.Action onComplete;
			
			public bool isDone { get; private set; }
			
			public Task(int niterations)
			{
				isDone = false;
			}

			public void Execute()
			{
				//usually this is an async operation (like www)
				//otherwise it would not make much sense :)
				isDone = true;

				if (onComplete != null)
					onComplete();
			}
		}

        #endregion

        #region EnumerableImplementation

		class Enumerable : IEnumerable
		{
			int totalIterations;
			int iterations;
			
			public event System.Action onComplete;
			
			public Enumerable(int niterations)
			{
				iterations = 0; 
				totalIterations = niterations;
			}
			
			public bool AllRight()
			{
				return iterations == totalIterations; 
			}
			
			public IEnumerator GetEnumerator()
			{
				while (iterations < totalIterations)
				{
					iterations++;
					
					yield return null;
				}
				
				if (onComplete != null)
					onComplete();
			}
		}

        #endregion

        #region Setup/Teardown
		
		TaskRunner _taskRunner;
		
		SerialTasks serialTasks1;
		SerialTasks serialTasks2;
		ParallelTasks parallelTasks1;
		ParallelTasks parallelTasks2;

		Task task1;
		Task task2;
		
		Enumerable iterable1;
		Enumerable iterable2;
		
        #endregion
		
		#region Helper functions
		
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
		
		#endregion		
	}
}
