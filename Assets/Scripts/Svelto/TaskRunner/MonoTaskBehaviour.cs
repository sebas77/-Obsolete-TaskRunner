using System;
using UnityEngine;

public class MonoTaskBehaviour: MonoBehaviour
{
	static public bool isQuitting = false;
	
	void OnApplicationQuit()
    {
        isQuitting = true;
    }
}

