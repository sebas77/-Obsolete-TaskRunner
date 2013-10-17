using System.Collections;
using System;
using UnityEngine;

namespace Svelto.Tasks
{
	public class YieldInstructionTask: IEnumerator
	{
		public object Current 		{ get { return _enumerator.Current; } }
				 
		public YieldInstructionTask(YieldInstruction instruction)
		{
			_enumerator = ConvertIt(instruction);
		}
		
		IEnumerator ConvertIt(YieldInstruction instruction)
		{
			yield return instruction;
		}
		
		virtual public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}
		public void Reset()
		{
			_enumerator.Reset();
		}
		
		private IEnumerator 		_enumerator;
	}
}
