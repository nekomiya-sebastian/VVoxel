using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( TimelineActor ) )]
public class TimelineActorEditor
	:
	Editor
{
	public enum SyncBehavior
	{
		ScrollToLatest, // scroll to latest possible frame
		FillInEmpty, // fill in empty frames
		LimitSelf // lock self to min possible frame
	}

	public enum AddFrameBehavior
	{
		DuplicateLatest, // dupe latest frame in keyframes array
		DuplicateCurrent, // dupe keyframes[curFrame]
		UseTransform, // create new frame & set targetPos & targetRot to transform pos & rot
		Blank // completely empty frame
	}

	public override void OnInspectorGUI()
	{
		if( Application.isPlaying ) return;

		var timelineActor = ( TimelineActor )target;

		if( timelineActor.keyframes.Count < 1 ) timelineActor.keyframes.Add( new TimelineKeyframe() );

		if( !cachedActors ) CacheOtherActors( timelineActor );

		int sliderMax = timelineActor.keyframes.Count - 1;
		int curSlider = timelineActor.TimelineFrame;
		string timelineText = "Timeline Frame | max: " + sliderMax;
		if( timelineActor.SyncBehavior == SyncBehavior.LimitSelf )
		{
			if( sliderMax > minSyncFrame ) sliderMax = minSyncFrame;
			if( curSlider > sliderMax ) curSlider = sliderMax;
			
			timelineText += " (limit " + minSyncFrame + ")";
		}

		EditorGUILayout.LabelField( timelineText );
		int prevFrame = timelineActor.TimelineFrame;
		timelineActor.TimelineFrame = EditorGUILayout.IntSlider( curSlider,0,sliderMax );
		
		GUILayout.BeginHorizontal();
		if( GUILayout.Button( "Prev Frame" ) ) timelineActor.ScrollFrame( -1 );

		if( GUILayout.Button( "Next Frame" ) ) timelineActor.ScrollFrame( 1 );
		GUILayout.EndHorizontal();

		if( prevFrame != timelineActor.TimelineFrame ) // copy src frame to frame view when scrolling
		{
			timelineActor.viewKeyframe.Set( timelineActor.keyframes[timelineActor.TimelineFrame] );

			if( timelineActor.AutoReadFrame ) timelineActor.LoadFromViewFrame();

			if( timelineActor.AutoSyncAll ) SyncOtherActors( timelineActor );
		}
		else // otherwise update src based on modified vals
		{
			if( timelineActor.AutoWriteFrame ) timelineActor.WriteFrame();
			else timelineActor.keyframes[timelineActor.TimelineFrame].Set( timelineActor.viewKeyframe );
		}
		
		if( GUILayout.Button( "Add Frame" ) ) timelineActor.AddFrame( AddFrameBehavior.DuplicateLatest );

		GUILayout.BeginHorizontal();
		bool prevAutoWrite = timelineActor.AutoWriteFrame;
		timelineActor.AutoWriteFrame = GUILayout.Toggle( timelineActor.AutoWriteFrame,"Auto Write Frame" );
		if( !prevAutoWrite && timelineActor.AutoWriteFrame ) timelineActor.WriteFrame();

		if( GUILayout.Button( "Write Frame" ) ) timelineActor.WriteFrame();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool prevAutoRead = timelineActor.AutoReadFrame;
		timelineActor.AutoReadFrame = GUILayout.Toggle( timelineActor.AutoReadFrame,"Auto Read Frame" );
		if( !prevAutoRead && timelineActor.AutoReadFrame ) timelineActor.LoadFromViewFrame();

		if( GUILayout.Button( "Reset Frame" ) ) timelineActor.LoadFromViewFrame();
		GUILayout.EndHorizontal();

		var prevSyncBehavior = timelineActor.SyncBehavior;
		timelineActor.SyncBehavior = ( SyncBehavior )EditorGUILayout.EnumPopup( "Sync Behavior",timelineActor.SyncBehavior );
		if( prevSyncBehavior != timelineActor.SyncBehavior ) CacheOtherActors( timelineActor );

		GUILayout.BeginHorizontal();
		bool prevAutoSync = timelineActor.AutoSyncAll;
		timelineActor.AutoSyncAll = GUILayout.Toggle( timelineActor.AutoSyncAll,"Auto Sync All" );
		if( !prevAutoSync && timelineActor.AutoSyncAll ) SyncOtherActors( timelineActor );

		if( GUILayout.Button( "Sync All Actors' Frames" ) ) SyncOtherActors( timelineActor );
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

	void SyncOtherActors( TimelineActor target )
	{
		CacheOtherActors( target );

		if( target.SyncBehavior == SyncBehavior.FillInEmpty )
		{
			int myFrame = target.TimelineFrame;
			foreach( var actor in actors )
			{
				int framesToAdd = myFrame - ( actor.keyframes.Count - 1 );
				for( int i = 0; i < framesToAdd; ++i ) actor.AddFrame( AddFrameBehavior.DuplicateLatest );
			}
		}

		foreach( var actor in actors )
		{
			actor.TimelineFrame = target.TimelineFrame;
			actor.LoadFromSrcFrame();
		}
	}

	void CacheOtherActors( TimelineActor self )
	{
		actors.Clear();
		var allActors = FindObjectsOfType<TimelineActor>();
		minSyncFrame = 9999;
		for( int i = 0; i < allActors.Length; ++i )
		{
			if( allActors[i] != self ) actors.Add( allActors[i] );

			if( minSyncFrame > allActors[i].keyframes.Count - 1 ) minSyncFrame = allActors[i].keyframes.Count - 1;
		}

		cachedActors = true;
	}

	bool canRemove = false;
	List<TimelineActor> actors = new List<TimelineActor>();
	bool cachedActors = false;
	int minSyncFrame = 9999;
}
