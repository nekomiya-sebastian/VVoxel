using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
	public Timer( float duration )
	{
		this.duration = duration;
	}

	public bool Update( float dt )
	{
		if( curTime <= duration ) curTime += dt;

		return( IsDone() );
	}

	public void Reset()
	{
		curTime = 0.0f;
	}

	public void SetDuration( float dur )
	{
		duration = dur;
	}

	public bool IsDone()
	{
		return( curTime >= duration );
	}

	public float GetPercent()
	{
		return( Mathf.Min( curTime / duration,1.0f ) );
	}

	public float GetDuration()
	{
		return( duration );
	}

	[SerializeField] float duration;
	float curTime = 0.0f;
}