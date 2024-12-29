using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineActor
	:
	MonoBehaviour
{
	public void WriteFrame()
	{
		viewKeyframe.targetPos = transform.position;
		viewKeyframe.targetRot = transform.eulerAngles;

		// keyframes[timelineFrame].targetPos = viewKeyframe.targetPos;
		// keyframes[timelineFrame].targetRot = viewKeyframe.targetRot;
		keyframes[TimelineFrame].Set( viewKeyframe );
	}

	public void LoadFrame()
	{
		transform.position = viewKeyframe.targetPos;
		transform.eulerAngles = viewKeyframe.targetRot;
	}

	public void ScrollFrame( int dir )
	{
		TimelineFrame += dir;
		if( TimelineFrame < 0 ) TimelineFrame = 0;
		if( TimelineFrame > keyframes.Count - 1 ) TimelineFrame = keyframes.Count - 1;
	}

	public int TimelineFrame { get; set; }
	public bool AutoReadFrame { get; set; }
	public bool AutoWriteFrame { get; set; }

	[SerializeField] public TimelineKeyframe viewKeyframe = new TimelineKeyframe();

	[Header( "Raw keyframes" )]
	[SerializeField] public List<TimelineKeyframe> keyframes = new List<TimelineKeyframe>();
}
