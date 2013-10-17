using System.Collections;

namespace Svelto.Tasks
{
    interface IRunner
    {
        void StartCoroutine(IEnumerator task);
        void StopAllCoroutines();
		void Destroy();
		
		bool paused { set; get; }
    }
}
