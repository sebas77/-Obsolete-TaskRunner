using System.Collections;
using System.Threading;

namespace Svelto.Tasks
{
    class MultiThreadRunner: IRunner
    {
        public void StartCoroutine(IEnumerator task)
        {	//this is not a background thread, the executable won't stop until this is done, it should be a background
            Thread oThread = new Thread(new ThreadStart(() => { while (task.MoveNext() == true); }));

            oThread.Start();
        }

        public void StopAllCoroutines()
        {
        }
		
		public void Destroy()
		{
		}
		
		public bool paused { set; get; }
    }
}
