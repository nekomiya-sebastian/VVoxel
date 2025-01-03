using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Animation behavior mostly lifted from AnimHandler
public class TimelineActor
	:
	MonoBehaviour
{
	void Start()
	{
		animCtrl = GetComponent<Animator>();

		curFrame = TimelineFrame;
		TimelineKeyframe.AnimState finalState = TimelineKeyframe.AnimState.Idle;
		TimelineKeyframe.AnimState lastValidState = TimelineKeyframe.AnimState.Idle;
		Vector3 finalPos = transform.position;
		Vector3 finalRot = transform.eulerAngles;
		for( int i = 0; i < TimelineFrame; ++i )
		{
			var frame = keyframes[i];

			if( !frame.emptyKeyframe )
			{
				finalState = frame.anim;
				if( frame.anim != TimelineKeyframe.AnimState.None ) lastValidState = frame.anim;

				finalPos = frame.targetPos;
				finalRot = frame.targetRot;

				foreach( var triggerObj in frame.triggerObjs ) triggerObj.GetComponent<AnimTriggerBase>().PerformAction();
			}
		}
		
		if( finalState != TimelineKeyframe.AnimState.None ) animCtrl.SetInteger( "State",( int )finalState );
		else // if( animCtrl != null )
		{
			// animCtrl.SetInteger( "State",( int )keyframes[curFrame - 1].anim );
			animCtrl.SetInteger( "State",( int )lastValidState );
			StartCoroutine( MaybeSetNoneTimer( -1 ) );
		}

		transform.position = finalPos;
		transform.eulerAngles = finalRot;
	}

	void Update()
	{
		if( !doneMoving )
		{
			if( moveTimer.Update( Time.deltaTime ) ) doneMoving = true;

			transform.position = Vector3.Lerp( startPos,keyframes[curFrame].targetPos,moveTimer.GetPercent() );
			transform.rotation = Quaternion.Lerp( startRot,targetRot,moveTimer.GetPercent() );
		}

		if( !doneAnim )
		{
			if( animTimer.Update( Time.deltaTime ) ) doneAnim = true;
		}
	}

	public void PerformAction()
	{
		if( !moveTimer.IsDone() )
		{
			transform.position = targetPos;
			transform.rotation = targetRot;
			doneMoving = true;
			doneAnim = true;
		}

		++curFrame;
		if( curFrame < keyframes.Count )
		{
			if( keyframes[curFrame].emptyKeyframe ) return;

			if( keyframes[curFrame].anim != TimelineKeyframe.AnimState.None )
			{
				animCtrl.speed = 1.0f;
				animCtrl.SetInteger( "State",( int )keyframes[curFrame].anim );
				
				StartCoroutine( MaybeSetNoneTimer() ); // idk why we need this here
			}

			foreach( var triggerObj in keyframes[curFrame].triggerObjs ) triggerObj.GetComponent<AnimTriggerBase>().PerformAction();

			startPos = transform.position;
			targetPos = keyframes[curFrame].targetPos;
			startRot = transform.rotation;
			targetRot = Quaternion.Euler( keyframes[curFrame].targetRot );
			
			moveTimer.SetDuration( keyframes[curFrame].moveDur );
			moveTimer.Reset();
			
			doneMoving = false;
			doneAnim = false;
			
			autoTransition = true;
			transType = keyframes[curFrame].transitionType;
			if( transType == TimelineKeyframe.TransitionType.TransitionAfterAnim ) StartCoroutine( SetAnimTimer() );
			else if( transType == TimelineKeyframe.TransitionType.TransitionAfterMove ) doneMoving = false;
			else if( transType == TimelineKeyframe.TransitionType.SyncMoveToAnim ) StartCoroutine( SetSyncMoveTimer() );
			else autoTransition = false;
		}
		else curFrame = keyframes.Count - 1;
	}

	public bool ReadyToTransition()
	{
		switch( transType )
		{
			case TimelineKeyframe.TransitionType.Manual: return( true );
			case TimelineKeyframe.TransitionType.SyncMoveToAnim: return( doneMoving && doneAnim );
			case TimelineKeyframe.TransitionType.TransitionAfterAnim: return( doneAnim );
			case TimelineKeyframe.TransitionType.TransitionAfterMove: return( doneMoving );
			default:
				Assert.IsTrue( false );
				return( false );
		}
	}

	public bool AutoTransition()
	{
		return( autoTransition );
	}

	IEnumerator SetAnimTimer()
	{
		yield return( new WaitForEndOfFrame() );
		animTimer.SetDuration( GetClipDur() );
		animTimer.Reset();
		doneAnim = false;
	}

	IEnumerator SetSyncMoveTimer()
	{
		yield return( new WaitForEndOfFrame() );
		moveTimer.SetDuration( GetClipDur() );
		moveTimer.Reset();
		doneMoving = false;
	}

	// use this to make sure the state doesn't reset if we don't want it to for non-looping anims (like fall over)
	IEnumerator MaybeSetNoneTimer( int actionOffset = 0 )
	{
		yield return( new WaitForEndOfFrame() );

		// this looks normal but the math is weird since this should technically be past action
		if( curFrame + actionOffset < keyframes.Count - 1 &&
			keyframes[curFrame + 1 + actionOffset].anim == TimelineKeyframe.AnimState.None )
		{
			yield return( new WaitForSeconds( GetClipDur() ) );
			animCtrl.speed = 0.0f;
			animCtrl.Play( animCtrl.GetCurrentAnimatorClipInfo( 0 )[0].clip.name,0,0.99f );
		}
	}

	float GetClipDur()
	{
		var clipInfo = animCtrl.GetCurrentAnimatorClipInfo( 0 )[0];
		if( animCtrl.GetNextAnimatorClipInfo( 0 ).Length > 0 )
		{
			clipInfo = animCtrl.GetNextAnimatorClipInfo( 0 )[0];
		}
		return( clipInfo.clip.length );
	}

	public void AddFrame( TimelineActorEditor.AddFrameBehavior behavior )
	{
		int dupeFrame = -1;
		var targetPos = Vector3.zero;
		var targetRot = Vector3.zero;
		switch( behavior )
		{
			case TimelineActorEditor.AddFrameBehavior.DuplicateLatest:
				for( int i = keyframes.Count - 1; i >= 0; --i )
				{
					if( !keyframes[i].emptyKeyframe )
					{
						dupeFrame = i;
						i = -1;
					}
				}
				break;
			case TimelineActorEditor.AddFrameBehavior.DuplicateCurrent:
				dupeFrame = curFrame;
				break;
			case TimelineActorEditor.AddFrameBehavior.UseTransform:
				targetPos = transform.position;
				targetRot = transform.eulerAngles;
				break;
			case TimelineActorEditor.AddFrameBehavior.Blank:
			default:
				// do nothing
				break;
		}

		if( dupeFrame > -1 )
		{
			targetPos = keyframes[dupeFrame].targetPos;
			targetRot = keyframes[dupeFrame].targetRot;
		}

		var frame = ( ( dupeFrame > -1 )
			? keyframes[dupeFrame].DuplicateKeyframe()
			: new TimelineKeyframe() );

		frame.targetPos = targetPos;
		frame.targetRot = targetRot;

		keyframes.Add( frame );
		TimelineFrame = keyframes.Count - 1;
		viewKeyframe.Set( frame );
	}

	public void WriteFrame()
	{
		viewKeyframe.targetPos = transform.position;
		viewKeyframe.targetRot = transform.eulerAngles;

		// keyframes[timelineFrame].targetPos = viewKeyframe.targetPos;
		// keyframes[timelineFrame].targetRot = viewKeyframe.targetRot;
		keyframes[TimelineFrame].Set( viewKeyframe );
	}

	public void LoadFromSrcFrame()
	{
		if( !keyframes[TimelineFrame].emptyKeyframe )
		{
			transform.position = keyframes[TimelineFrame].targetPos;
			transform.eulerAngles = keyframes[TimelineFrame].targetRot;
		}
	}

	public void LoadFromViewFrame()
	{
		if( !viewKeyframe.emptyKeyframe )
		{
			transform.position = viewKeyframe.targetPos;
			transform.eulerAngles = viewKeyframe.targetRot;
		}
	}

	public void ScrollFrame( int dir )
	{
		TimelineFrame += dir;
		if( TimelineFrame < 0 ) TimelineFrame = 0;
		if( TimelineFrame > keyframes.Count - 1 ) TimelineFrame = keyframes.Count - 1;
	}

	public int TimelineFrame
	{
		get { return( curFrame ); }
		set
		{
			curFrame = value;
			if( curFrame < 0 ) curFrame = 0;
			else if( curFrame > keyframes.Count - 1 ) curFrame = keyframes.Count - 1;
		}
	}
	public bool AutoReadFrame
	{
		get { return( autoRead ); }
		set { autoRead = value; }
	}
	public bool AutoWriteFrame
	{
		get { return( autoWrite ); }
		set { autoWrite = value; }
	}
	public bool AutoSyncAll
	{
		get { return( autoSync ); }
		set { autoSync = value; }
	}
	public TimelineActorEditor.SyncBehavior SyncBehavior
	{
		get { return( syncBehavior ); }
		set { syncBehavior = value; }
	}

	[SerializeField] public TimelineKeyframe viewKeyframe = new TimelineKeyframe();

	[HideInInspector] [SerializeField] public List<TimelineKeyframe> keyframes = new List<TimelineKeyframe>();

	Animator animCtrl;

	[HideInInspector] [SerializeField] int curFrame = 0;
	[HideInInspector] [SerializeField] bool autoRead = false;
	[HideInInspector] [SerializeField] bool autoWrite = false;
	[HideInInspector] [SerializeField] bool autoSync = false;
	[HideInInspector] [SerializeField] TimelineActorEditor.SyncBehavior syncBehavior = TimelineActorEditor.SyncBehavior.ScrollToLatest;

	Timer animTimer = new Timer( 0.0f );
	Timer moveTimer = new Timer( 0.0f );
	bool doneAnim = true;
	bool doneMoving = true;
	bool autoTransition = false;

	Vector3 startPos = Vector3.zero;
	Vector3 targetPos = Vector3.zero;
	Quaternion startRot = Quaternion.identity;
	Quaternion targetRot = Quaternion.identity;

	TimelineKeyframe.TransitionType transType = TimelineKeyframe.TransitionType.Manual;
}
