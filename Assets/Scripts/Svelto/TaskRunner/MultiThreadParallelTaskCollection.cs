using Svelto.Tasks.Internal;
using System;
using System.Collections;
using System.Threading;
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
#endif

namespace Svelto.Tasks
{
    /// <summary>
    /// a ParallelTaskCollection ran by MultiThreadRunner will run the tasks in a single thread
    /// MultiThreadParallelTaskCollection enables parallel tasks to run on different threads
    /// </summary>
    public class MultiThreadParallelTaskCollection : TaskCollection
    {
        const int MAX_CONCURRENT_TASK = 8;

        public event Action onComplete;

        override public float progress { get { return _progress; } }

        public MultiThreadParallelTaskCollection(MultiThreadRunner runner) : base()
        {
            _maxConcurrentTasks = uint.MaxValue;
            _taskRoutinePool = new TaskRoutinePool(runner);

            ComputeMaxConcurrentTasks();
        }

        public MultiThreadParallelTaskCollection(MultiThreadRunner runner, uint maxConcurrentTasks) : this(runner)
        {
            _maxConcurrentTasks = Math.Min(MAX_CONCURRENT_TASK, _maxConcurrentTasks);
        }

        private void ComputeMaxConcurrentTasks()
        {
            if (_maxConcurrentTasks != uint.MaxValue)
                _maxConcurrentTasks = Math.Min(_maxConcurrentTasks, (uint)(registeredEnumerators.Count));
        }

        override public IEnumerator GetEnumerator()
        {
            _startingCount = registeredEnumerators.Count;

            if (_startingCount > 0)
            {
                isRunning = true;

                RunMultiThreadParallelTasks();

                isRunning = false;
            }

            if (onComplete != null)
                onComplete();

            yield return null;
        }

        void OnThreadedTaskDone()
        {
            lock (_locker)
            {
                --_counter;
               _progress = (float)(_startingCount - registeredEnumerators.Count) / (float)_startingCount;

                Monitor.Pulse(_locker);
            }
            
            _countdown.Signal();
        }

        void RunMultiThreadParallelTasks()
        {
            _counter = 0;

            _countdown.AddCount(registeredEnumerators.Count);

            while (registeredEnumerators.Count > 0)
            {
                _taskRoutinePool.RetrieveTask().Start(RunTask(), false);
                                
                lock (_locker)
                {
                    if (++_counter >= _maxConcurrentTasks)
                        Monitor.Wait(_locker);
                }
            }

            _countdown.Wait();
        }

        private IEnumerator RunTask()
        {
            yield return registeredEnumerators.Dequeue();

            OnThreadedTaskDone();
        }

        volatile float _progress;
        volatile float _totalTasks;

        uint                _maxConcurrentTasks;
        object              _locker = new object();
        Countdown           _countdown = new Countdown();
        volatile int        _counter = 0;
        int                 _startingCount;

        TaskRoutinePool     _taskRoutinePool;
    }

    public class Countdown
    {
        object _locker = new object();
        int _value;

        public Countdown() { }
        public Countdown(int initialCount) { _value = initialCount; }

        public void Signal() { AddCount(-1); }

        public void AddCount(int amount)
        {
            lock (_locker)
            {
                _value += amount;
                if (_value <= 0) Monitor.PulseAll(_locker);
            }
        }

        public void Wait()
        {
            lock (_locker)
              while (_value > 0)
                    Monitor.Wait(_locker);
        }
    }
}
