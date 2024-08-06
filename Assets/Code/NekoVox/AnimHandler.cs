using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class AnimHandler
	:
	MonoBehaviour
{
	public enum AnimState
	{
		None = -1,
		Idle,
		Wave,
		Sit,
		Run,
		Jump,
		Talk,
		Leap,
		No,
		Shoot,
		Collapse,
		HeadShake,
		Wave2,
		HandsOnHips,
		LieDown,
		Kneel,
		HeadTilt,
		Count
	}

	[System.Serializable]
	class AnimAction
	{
		[SerializeField] public AnimState animState = AnimState.None;
		[SerializeField] public Vector3 targetPos = Vector3.zero;
		[SerializeField] public bool move = false;
		[SerializeField] public float moveDur = 0.0f;
		[SerializeField] public Vector3 targetRot = Vector3.zero;
		[SerializeField] public bool rotate = false;
		[SerializeField] public bool instantTransition = false;
		[SerializeField] public bool transitionAfterAnim = false;
		[SerializeField] public GameObject triggerObj = null;
	}

	void Start()
	{
		animCtrl = GetComponent<Animator>();

		if( startAction >= 0 )
		{
			curAction = startAction;

			AnimState finalState = AnimState.Idle;
			Vector3 finalPos = transform.position;
			Vector3 finalRot = transform.eulerAngles;
			// foreach( var ac in actions )
			for( int i = 0; i < startAction + 1; ++i )
			{
				var ac = actions[i];

				finalState = ac.animState;

				if( ac.move ) finalPos = ac.targetPos;
				if( ac.rotate ) finalRot = ac.targetRot;

				if( ac.triggerObj != null ) ac.triggerObj.GetComponent<AnimTriggerBase>().PerformAction();
			}
			
			if( finalState != AnimState.None ) animCtrl.SetInteger( "State",( int )finalState );
			else if( animCtrl != null )
			{
				animCtrl.SetInteger( "State",( int )actions[curAction - 1].animState );
				StartCoroutine( MaybeSetNoneTimer( -1 ) );
			}
			transform.position = finalPos;
			transform.eulerAngles = finalRot;
		}
	}

	void Update()
	{
		if( printAction && curAction >= actions.Count - 1 && Input.GetKeyDown( KeyCode.Space ) )
		{
			PrintCurAction();
			++curAction;
		}

		if( doneMoving && doneAnim && curAction < actions.Count - 1 )
		{
			if( ( Input.GetKeyDown( KeyCode.Space ) ) ||
				( curAction >= 0 && actions[curAction].instantTransition ) ||
				( curAction >= 0 && actions[curAction].transitionAfterAnim ) )
			{
				if( printAction ) PrintCurAction();
				PerformAction();
			}
		}
		
		if( !doneMoving )
		{
			if( moveTimer.Update( Time.deltaTime ) )
			{
				doneMoving = true;
			}

			if( actions[curAction].move )
			{
				transform.position = Vector3.Lerp( startPos,actions[curAction].targetPos,moveTimer.GetPercent() );
			}
			if( actions[curAction].rotate )
			{
				transform.rotation = Quaternion.Lerp( startRot,targetRot,moveTimer.GetPercent() );
			}
		}

		if( !doneAnim )
		{
			if( animTimer.Update( Time.deltaTime ) ) doneAnim = true;
		}
	}

	public void PerformAction()
	{
		++curAction;
		if( curAction < actions.Count )
		{
			if( actions[curAction].animState != AnimState.None )
			{
				animCtrl.speed = 1.0f;
				animCtrl.SetInteger( "State",( int )actions[curAction].animState );
				
				StartCoroutine( MaybeSetNoneTimer() );
			}
			// animCtrl.Play( "Base Layer." + actions[curAction].anim.name );

			if( actions[curAction].triggerObj != null )
			{
				actions[curAction].triggerObj.GetComponent<AnimTriggerBase>().PerformAction();
			}

			if( actions[curAction].move || actions[curAction].rotate )
			{
				doneMoving = false;
				if( actions[curAction].move ) startPos = transform.position;
				if( actions[curAction].rotate )
				{
					startRot = transform.rotation;
					targetRot = Quaternion.Euler( actions[curAction].targetRot );
				}
				moveTimer.SetDuration(actions[curAction].moveDur );
				moveTimer.Reset();
			}

			if( actions[curAction].transitionAfterAnim ) StartCoroutine( SetAnimTimer() );
		}
	}

	void PrintCurAction()
	{
		print( gameObject.name + ": " + ( curAction + 1 ).ToString() );
	}

	IEnumerator SetAnimTimer()
	{
		yield return( new WaitForEndOfFrame() );
		animTimer.SetDuration( GetClipDur() );
		animTimer.Reset();
		doneAnim = false;
	}

	IEnumerator MaybeSetNoneTimer( int actionOffset = 0 )
	{
		yield return( new WaitForEndOfFrame() );

		// this looks normal but the math is weird since this should technically be past action
		if( curAction + actionOffset < actions.Count - 1 && actions[curAction + 1 + actionOffset].animState == AnimState.None )
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

	Animator animCtrl;

	[SerializeField] bool printAction = false;

	[SerializeField] int startAction = -1;

	[Header( "Actions" )]
	[SerializeField] List<AnimAction> actions = new List<AnimAction>();
	int curAction = -1;

	bool doneMoving = true;
	bool doneAnim = true;
	Vector3 startPos = Vector3.zero;
	Quaternion startRot = Quaternion.identity;
	Quaternion targetRot = Quaternion.identity;
	Timer moveTimer = new Timer( 0.0f );
	Timer animTimer = new Timer( 0.0f );
}