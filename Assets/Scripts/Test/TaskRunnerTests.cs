#region Usings
using System.Collections;
using Svelto.Tasks;
using NUnit.Framework;
#endregion

//Note: RunSync is used only for testing purposes
//Real scenarios should use Run or RunManaged

namespace Test
{
	[TestFixture]
    public class TaskRunnerTests
	{
		[SetUp]
		public void Setup ()
		{
			serialTasks1 = new SerialTaskCollection();
			parallelTasks1 = new ParallelTaskCollection();
			serialTasks2 = new SerialTaskCollection();
			parallelTasks2 = new ParallelTaskCollection();

			task1 = new Task(15);
			task2 = new Task(5);
			
			iterable1 = new Enumerable(15);
			iterable2 = new Enumerable(5);
			
			iterations = 0;
			
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
			
			_taskRunner.RunSync (serialTasks1.GetEnumerator());

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
			
			_taskRunner.RunSync (parallelTasks1.GetEnumerator());
			
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

			task1.OnComplete((b) => {
				test1Done = true; });
			
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
			
			task1.OnComplete((b) => {	test1Done = true; });
			task2.OnComplete((b) => { Assert.That (test1Done == true); });
			
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
			
			_taskRunner.RunSync (serialTasks1.GetEnumerator());
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

			_taskRunner.RunSync (serialTasks1.GetEnumerator());

			Assert.That (parallelTasks1Done == true, "parallelTasks1Done");
			Assert.That (parallelTasks2Done == true, "parallelTasks2Done");
			Assert.That (allDone == true, "allDone");
		}
		
		[Test]
		public void TestSerialTasks1ExecutedInParallel ()
		{
			bool serialTasks1Done = false;

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);
			serialTasks1.onComplete += () => {serialTasks1Done = true; };

			serialTasks2.Add (task1);
			serialTasks2.Add (task2);

			parallelTasks1.Add (serialTasks1);
			parallelTasks1.Add (serialTasks2);

			_taskRunner.RunSync (parallelTasks1.GetEnumerator());

			Assert.That (serialTasks1Done == true);
		}
		
		[Test]
		public void TestSerialTasks2ExecutedInParallel ()
		{
			bool serialTasks2Done = false;

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);

			serialTasks2.Add (task1);
			serialTasks2.Add (task2);
			serialTasks2.onComplete += () => {serialTasks2Done = true; };

			parallelTasks1.Add (serialTasks1);
			parallelTasks1.Add (serialTasks2);

			_taskRunner.RunSync (parallelTasks1.GetEnumerator());

			Assert.That (serialTasks2Done == true);
		}

		[Test]
		public void TestSerialTasksAreAllExecutedInParallel ()
		{
			bool allDone = false;

			serialTasks1.Add (iterable1);
			serialTasks1.Add (iterable2);

			serialTasks2.Add (task1);
			serialTasks2.Add (task2);

			parallelTasks1.Add (serialTasks1);
			parallelTasks1.Add (serialTasks2);
			parallelTasks1.onComplete += () => {allDone = true; };

			_taskRunner.RunSync (parallelTasks1.GetEnumerator());

			Assert.That (allDone == true);
		}
		
		[Test]
		public void TestSerialTasksExecutedInParallelWithEnumFunction ()
		{
			_taskRunner.RunSync (EnumeratorFunction());

			Assert.That (iterations == 10);
		}
		
        #region TaskImplementation

		class Task : ITask
		{
			event System.Action<bool> _onComplete;

			public IAbstractTask	OnComplete(System.Action<bool> action)
			{
				_onComplete += action;

				return this;
			}
			
			public bool  isDone { get; private set; }
			public float progress { get; private set; }
			
			public Task(int niterations)
			{
				isDone = false;
				progress = 0.0f;
			}

			public void Execute()
			{
				//usually this is an async operation (like www)
				//otherwise it would not make much sense :)
				isDone = true;
				progress = 1.0f;

				if (_onComplete != null)
					_onComplete(true);
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
		
		SerialTaskCollection serialTasks1;
		SerialTaskCollection serialTasks2;
		ParallelTaskCollection parallelTasks1;
		ParallelTaskCollection parallelTasks2;

		Task task1;
		Task task2;
		
		Enumerable iterable1;
		Enumerable iterable2;
		
		int	iterations;
		
        #endregion
		
		#region Helper functions
		
		void SetupAndRunSerialTasks ()
		{
			serialTasks1.Add (task1);
			serialTasks1.Add (task2);
			
			_taskRunner.RunSync (serialTasks1.GetEnumerator());
		}

		void SetupAndRunParallelTasks ()
		{
			parallelTasks1.Add (task1);
			parallelTasks1.Add (task2);
			
			_taskRunner.RunSync (parallelTasks1.GetEnumerator());
		}
		
		IEnumerator EnumeratorFunction ()
		{
			while (iterations < 10)
			{
				iterations++;
					
				yield return null;
			}
		}
		
		#endregion		
	}
}
