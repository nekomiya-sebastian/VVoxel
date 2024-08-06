using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class CustomCharCtrl
	:
	MonoBehaviour
{
	void Start()
	{
		cam = Camera.main;
		eventSys = EventSystem.current;
		rayMask = LayerMask.GetMask( "Default" );
		uiMask = LayerMask.GetMask( "UI" );

		Assert.IsNotNull( partsPanel );
	}

	void Update()
	{
		bool hoveringMenu = false;
		if( !moving && !rotating )
		{
			var pointerData = new PointerEventData( eventSys );
			pointerData.position = Input.mousePosition;
			var results = new List<RaycastResult>();
			eventSys.RaycastAll( pointerData,results );
			// foreach( var result in results ) print( result );
			hoveringMenu = ( results.Count > 0 );
		}

		{
			var mousePos = ( Vector2 )Input.mousePosition;
			
			var diff = mousePos - mouseStart;

			// rotation
			if( Input.GetMouseButton( 1 ) )
			{
				if( !hoveringMenu && !locked )
				{
					if( !movePart )
					{
						var rot = transform.eulerAngles;
						rot.y -= diff.x * rotSpd.y;
						transform.eulerAngles = rot;
						transform.Rotate( Vector3.right,-diff.y * rotSpd.x,Space.World );
					}
					else
					{
						var rot = targetPart.eulerAngles;
						rot.y -= diff.x * partRotSpd.y;
						targetPart.eulerAngles = rot;
						targetPart.Rotate( Vector3.right,-diff.y * partRotSpd.x,Space.World );

						SaveLoadHandler.UpdateRot( targetPart.localEulerAngles );
					}
					rotating = true;
				}
			}
			else rotating = false;

			// movement
			if( Input.GetMouseButton( 0 ) )
			{
				if( !hoveringMenu && !locked )
				{
					if( !movePart )
					{
						var move = Vector3.right * diff.x * -1.0f * moveSpd + Vector3.up * diff.y * moveSpd;
						transform.Translate( move,Space.World );
					}
					else
					{
						var move = Vector3.right * diff.x * -1.0f * partMoveSpd + Vector3.up * diff.y * partMoveSpd;
						targetPart.Translate( move,Space.World );
						
						SaveLoadHandler.UpdateOffset( targetPart.localPosition );
					}
					moving = true;
				}
			}
			else moving = false;

			mouseStart = mousePos;

			// deselect part when clicking off of character
			// if( diff == Vector2.zero && !hoveringMenu && Input.GetMouseButtonUp( 0 ) &&
			// 	!moving && partsPanel.HasOpenPanel() )
			// {
			// 	partsPanel.Deselect();
			// }

			// select part
			if( !hoveringMenu && diff == Vector2.zero && Input.GetMouseButtonUp( 0 ) )
			{
				// var ray = new Ray( cam.transform.position,cam.transform.forward );
				var ray = cam.ScreenPointToRay( Input.mousePosition );
				RaycastHit hit;
				if( Physics.Raycast( ray,out hit,20.0f,rayMask ) )
				{
					var transStart = hit.transform;
					while( !transStart.name.Contains( "Pivot" ) && transStart.parent != null )
					{
						transStart = transStart.parent;
					}
					if( transStart.name.Contains( "Pivot" ) ) partsPanel.TryOpenPanel( transStart.name );
				}
			}
		}
			
		// zoom/scale
		if( Mathf.Abs( Input.mouseScrollDelta.y ) > 0.0f && !hoveringMenu && !locked )
		{
			var scale = transform.localScale;

			float newScroll = 0.0f;
			if( !movePart )
			{
				newScroll = scale.x + ( Input.mouseScrollDelta.y > 0.0f ? scrollSpd : -scrollSpd );
			}
			else
			{
				scale = targetPart.localScale;
				newScroll = scale.x + ( Input.mouseScrollDelta.y > 0.0f ? partScrollSpd : -partScrollSpd );
			}

			if( newScroll > scrollSpd )
			{
				scale = Vector3.one * newScroll;
				if( !movePart ) transform.localScale = scale;
				else targetPart.localScale = scale;
			}
		}
	}

	public void SetMovePart( bool move,Transform targetPart )
	{
		movePart = move;
		SetTargetPart( targetPart );
	}

	public void SetTargetPart( Transform targetPart )
	{
		this.targetPart = targetPart;
	}

	public void LockMovement( bool locked )
	{
		this.locked = locked;
	}

	Camera cam;
	EventSystem eventSys;
	LayerMask rayMask;
	LayerMask uiMask;

	Vector2 mouseStart = Vector2.zero;
	bool moving = false;
	bool rotating = false;

	[SerializeField] PartsPanel partsPanel = null;

	[Header( "Translation Speed")]
	[SerializeField] float moveSpd = 1.0f;
	[SerializeField] Vector2 rotSpd = Vector2.one;
	[SerializeField] float scrollSpd = 0.1f;

	[Header( "Part Translation Speed")]
	[SerializeField] float partMoveSpd = 1.0f;
	[SerializeField] Vector2 partRotSpd = Vector2.one;
	[SerializeField] float partScrollSpd = 0.1f;

	bool movePart = false;
	Transform targetPart = null;
	bool locked = false;
}