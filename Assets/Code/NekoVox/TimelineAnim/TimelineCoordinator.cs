using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineCoordinator
	:
	MonoBehaviour
{
	void Start()
	{
		actors = FindObjectsOfType<TimelineActor>();
	}

	void Update()
	{
		bool canProgress = true;
		bool autoProgress = false;

		for( int i = 0; i < actors.Length; ++i )
		{
			if( !actors[i].ReadyToTransition() ) canProgress = false;
			else if(actors[i].AutoTransition() ) autoProgress = true;
		}

		if( canProgress )
		{
			if( Input.GetKeyDown( KeyCode.Space ) || autoProgress )
			{
				for( int i = 0; i < actors.Length; ++i ) actors[i].PerformAction();
			}
		}
	}

	TimelineActor[] actors;
}
