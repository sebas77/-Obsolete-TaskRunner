using System;
using UnityEngine;

public class MonoTaskBehaviour: MonoBehaviour
{
	public Action onLevelWasLoaded;
	
	void OnLevelWasLoaded (int level) 
	{
		if (onLevelWasLoaded != null)
		{
			onLevelWasLoaded();
		}
	}
}

