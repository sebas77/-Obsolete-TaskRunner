using UnityEngine;

class MonoTask : MonoBehaviour
{
	public bool paused { set; get; }
	
	void Awake()
	{
		paused = false;	
	}
}