using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( TimelineActor ) )]
public class TimelineActorEditor
	:
	Editor
{
	public override void OnInspectorGUI()
	{
		if( Application.isPlaying ) return;

		var timelineActor = ( TimelineActor )target;

		if( timelineActor.keyframes.Count < 1 ) timelineActor.keyframes.Add( new TimelineKeyframe() );

		EditorGUILayout.LabelField( "Timeline Frame" );
		int prevFrame = timelineActor.TimelineFrame;
		timelineActor.TimelineFrame = EditorGUILayout.IntSlider( timelineActor.TimelineFrame,0,timelineActor.keyframes.Count - 1 );
		
		GUILayout.BeginHorizontal();
		if( GUILayout.Button( "Prev Frame" ) ) timelineActor.ScrollFrame( -1 );

		if( GUILayout.Button( "Next Frame" ) ) timelineActor.ScrollFrame( 1 );
		GUILayout.EndHorizontal();

		if( prevFrame != timelineActor.TimelineFrame ) // copy src frame to frame view when scrolling
		{
			timelineActor.viewKeyframe.Set( timelineActor.keyframes[timelineActor.TimelineFrame] );

			if( timelineActor.AutoReadFrame ) timelineActor.LoadFrame();
		}
		else // otherwise update src based on modified vals
		{
			if( timelineActor.AutoWriteFrame ) timelineActor.WriteFrame();
			else timelineActor.keyframes[timelineActor.TimelineFrame].Set( timelineActor.viewKeyframe );
		}

		bool prevAutoWrite = timelineActor.AutoWriteFrame;
		timelineActor.AutoWriteFrame = GUILayout.Toggle( timelineActor.AutoWriteFrame,"Auto Write Frame" );
		if( !prevAutoWrite && timelineActor.AutoWriteFrame ) timelineActor.WriteFrame();

		bool prevAutoRead = timelineActor.AutoReadFrame;
		timelineActor.AutoReadFrame = GUILayout.Toggle( timelineActor.AutoReadFrame,"Auto Read Frame" );
		if( !prevAutoRead && timelineActor.AutoReadFrame ) timelineActor.LoadFrame();
		
		if( GUILayout.Button( "Add Frame" ) )
		{
			var frame = ( timelineActor.keyframes.Count > 0
				? timelineActor.keyframes[timelineActor.keyframes.Count - 1].DuplicateKeyframe()
				: new TimelineKeyframe() );

			frame.targetPos = timelineActor.transform.position;
			frame.targetRot = timelineActor.transform.eulerAngles;

			timelineActor.keyframes.Add( frame );
			timelineActor.TimelineFrame = timelineActor.keyframes.Count - 1;
			timelineActor.viewKeyframe.Set( frame );
		}

		GUILayout.BeginHorizontal();
		if( GUILayout.Button( "Write Frame" ) ) timelineActor.WriteFrame();

		if( GUILayout.Button( "Reset Frame" ) ) timelineActor.LoadFrame();
		GUILayout.EndHorizontal();

		canRemove = GUILayout.Toggle( canRemove,"Remove All Frames Safety Switch" );
		if( canRemove && GUILayout.Button( "Remove All Frames" ) )
		{
			timelineActor.TimelineFrame = 0;
			timelineActor.viewKeyframe = new TimelineKeyframe();
			timelineActor.keyframes.Clear();
			timelineActor.keyframes.Add( new TimelineKeyframe() );
		}

		DrawDefaultInspector();
	}

	bool canRemove = false;
}
