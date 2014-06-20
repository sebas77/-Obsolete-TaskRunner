using System.Collections;

namespace Svelto.Tasks
{
	public interface IRunner
    {
		void	StartCoroutine(IEnumerator task);
		void 	StartCoroutine(IEnumerable task);
        void 	StopAllCoroutines();
				
		bool paused { get; set; }
		bool stopped { get; }
    }
}
