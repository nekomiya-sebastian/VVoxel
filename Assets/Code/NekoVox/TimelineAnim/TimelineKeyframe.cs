using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimelineKeyframe
{
	public enum TransitionType
	{
		Manual,
		TransitionAfterAnim,
		TransitionAfterMove
	}

	[Tooltip( "If true, removes fields & changes nothing this frame (just acts as a filler while other actors are acting" )]
	[SerializeField] public bool emptyKeyframe = false;

	[Tooltip( "Animation to play when this keyframe is reached" )]
	[SerializeField] public AnimHandler.AnimState anim = AnimHandler.AnimState.None;
	[Tooltip( "Position to move to" )]
	[SerializeField] public Vector3 targetPos = Vector3.zero;
	[Tooltip( "Rotation to rotate to" )]
	[SerializeField] public Vector3 targetRot = Vector3.zero;

	[Tooltip( "Duration of movement & rotation to target pos & rot" )]
	[SerializeField] public float moveDur = 0.0f;

	[Tooltip( "AnimTrigger objects to call PerformAction on")]
	[SerializeField] public List<GameObject> triggerObjs = new List<GameObject>();

	[Tooltip( "Manual = transition when press space, & is default for empty keyframes")]
	[SerializeField] public TransitionType transitionType = TransitionType.Manual;

	public void Set( TimelineKeyframe refFrame )
	{
		emptyKeyframe = refFrame.emptyKeyframe;

		anim = refFrame.anim;
		targetPos = refFrame.targetPos;
		targetRot = refFrame.targetRot;
		
		moveDur = refFrame.moveDur;

		foreach( var obj in refFrame.triggerObjs ) triggerObjs.Add( obj );

		transitionType = refFrame.transitionType;
	}

	public TimelineKeyframe DuplicateKeyframe()
	{
		var copy = new TimelineKeyframe();

		copy.Set( this );

		return( copy );
	}
}
